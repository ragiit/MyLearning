using BuildingBlocks.CQRS;
using FluentValidation;
using Basket.API.Data;
using Basket.API.Dtos;
using Basket.API.Exceptions;
using Microsoft.Extensions.Logging; // Tambahkan ini
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Basket.API.Services; // Tambahkan ini untuk IMenuApi
using Refit; // Tambahkan ini untuk ApiException

namespace Basket.API.Features.UpdateBasketItemQuantity
{
    // COMMAND dan VALIDATOR tetap sama
    public sealed record UpdateBasketItemQuantityCommand(UpdateBasketItemQuantityRequest Request, string UserName) : ICommand<BasketDto>;

    public class UpdateBasketItemQuantityCommandValidator : AbstractValidator<UpdateBasketItemQuantityCommand>
    {
        public UpdateBasketItemQuantityCommandValidator()
        {
            RuleFor(x => x.Request.MenuId).NotEmpty().WithMessage("MenuId diperlukan.");
            RuleFor(x => x.Request.Quantity).InclusiveBetween(0, int.MaxValue).WithMessage("Kuantitas harus berupa angka non-negatif.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("ID Pengguna diperlukan.");
        }
    }

    // HANDLER
    public class UpdateBasketItemQuantityHandler(
        IBasketRepository repository,
        IMenuApiClient menuApi, // Injeksi IMenuApi
        ILogger<UpdateBasketItemQuantityHandler> logger) : ICommandHandler<UpdateBasketItemQuantityCommand, BasketDto>
    {
        public async Task<BasketDto> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
        {
            // 1. Dapatkan keranjang yang ada
            var basket = await repository.GetBasketAsync(request.UserName, cancellationToken)
                ?? throw new BasketNotFoundException(request.UserName);

            var existingItem = basket.Items.FirstOrDefault(item => item.MenuId == request.Request.MenuId);

            if (existingItem == null)
            {
                throw new BasketItemNotFoundException(request.Request.MenuId);
            }

            // 2. Dapatkan detail Menu dari Menu Microservice
            // Ini penting untuk memverifikasi harga dan ketersediaan terbaru
            BaseResponse<MenuDto>? menuApiResponse = null;
            try
            {
                // Panggil method interface Refit. Token akan dipropagasi oleh DelegatingHandler.
                menuApiResponse = await menuApi.GetMenuById(request.Request.MenuId, "", cancellationToken);
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, "Gagal mendapatkan detail menu dari Menu API via Refit untuk MenuId {MenuId}. Status Code: {StatusCode}. Konten: {Content}", request.Request.MenuId, ex.StatusCode, ex.Content);
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new MenuNotFoundException(request.Request.MenuId); // Lempar jika menu tidak ditemukan
                }
                throw; // Lempar ulang exception lain
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Terjadi kesalahan tak terduga saat mendapatkan detail menu dari Menu API via Refit untuk MenuId {MenuId}", request.Request.MenuId);
                throw;
            }

            var menuDetails = menuApiResponse?.Result;
            if (menuDetails == null)
            {
                // Ini seharusnya sudah ditangkap oleh ApiException 404, tapi sebagai fallback
                logger.LogWarning("Item menu dengan ID {MenuId} tidak ditemukan di Menu API setelah pemanggilan.", request.Request.MenuId);
                throw new MenuNotFoundException(request.Request.MenuId);
            }

            // 3. Perbarui item keranjang
            // Hapus item lama dari daftar
            basket.Items.Remove(existingItem);

            if (request.Request.Quantity == 0)
            {
                logger.LogInformation("Menghapus item {MenuName} (ID: {MenuId}) dari keranjang pengguna {UserName} karena kuantitas 0.", menuDetails.Name, request.Request.MenuId, request.UserName);
                // Tidak perlu menambahkannya kembali jika kuantitas 0
            }
            else
            {
                // Tambahkan kembali dengan kuantitas baru dan harga/gambar terbaru dari Menu Service
                basket.Items.Add(existingItem with
                {
                    Quantity = request.Request.Quantity,
                    Price = menuDetails.Price, // Update harga ke harga terbaru dari Menu Service
                    ImageUrl = menuDetails.ImageUrl, // Update URL gambar ke yang terbaru
                    MenuName = menuDetails.Name // Update nama menu ke yang terbaru
                });
                logger.LogInformation("Kuantitas item {MenuName} (ID: {MenuId}) di keranjang pengguna {UserName} diperbarui menjadi {Quantity}.", menuDetails.Name, request.Request.MenuId, request.UserName, request.Request.Quantity);
            }

            // 4. Hitung ulang total harga
            basket = basket with { TotalPrice = basket.Items.Sum(item => item.Price * item.Quantity) };

            // 5. Simpan keranjang yang diperbarui
            await repository.StoreBasketAsync(basket, cancellationToken);

            return basket;
        }
    }
}