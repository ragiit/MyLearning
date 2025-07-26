using Refit;

namespace Basket.API.Features.AddItemToBasket
{
    // COMMAND
    public sealed record AddItemToBasketCommand(AddItemToBasketRequest Request, string UserName) : ICommand<BasketDto>;

    // VALIDATOR
    public class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
    {
        public AddItemToBasketCommandValidator()
        {
            RuleFor(x => x.Request.MenuId).NotEmpty().WithMessage("MenuId is required.");
            RuleFor(x => x.Request.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }

    // HANDLER
    public class AddItemToBasketHandler(
        IBasketRepository repository,
        IMenuApiClient menuApiClient, // Untuk panggil Menu Service
        ILogger<AddItemToBasketHandler> logger) : ICommandHandler<AddItemToBasketCommand, BasketDto>
    {
        public async Task<BasketDto> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Adding item to basket for user {UserName}", request.UserName);

            // 1. Get existing basket or create a new one
            var basket = await repository.GetBasketAsync(request.UserName, cancellationToken);
            if (basket == null)
            {
                basket = new BasketDto(request.UserName, [], 0);
                logger.LogInformation("New basket created for user {UserName}", request.UserName);
            }

            BaseResponse<MenuDto>? menuApiResponse = null;
            try
            {
                // Panggil method interface Refit
                // Propagasi token dilakukan secara otomatis oleh AuthorizationHeaderHandler
                menuApiResponse = await menuApiClient.GetMenuById(request.Request.MenuId, "", cancellationToken); // Authorization header diisi handler
            }
            catch (ApiException ex) // Refit akan melempar ApiException untuk status code 4xx/5xx
            {
                logger.LogError(ex, "Gagal mendapatkan detail menu dari Menu API via Refit untuk MenuId {MenuId}. Status Code: {StatusCode}. Konten: {Content}", request.Request.MenuId, ex.StatusCode, ex.Content);
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new MenuNotFoundException(request.Request.MenuId);
                }
                throw; // Lempar ulang exception lain
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Terjadi kesalahan tak terduga saat mendapatkan detail menu dari Menu API via Refit untuk MenuId {MenuId}", request.Request.MenuId);
                throw;
            }

            var menuDetails = menuApiResponse?.Result;

            // 3. Update basket items
            var existingItem = basket.Items.FirstOrDefault(item => item.MenuId == request.Request.MenuId);

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + request.Request.Quantity;
                basket.Items.Remove(existingItem);
                basket.Items.Add(existingItem with { Quantity = newQuantity }); // Record with syntax for immutability
            }
            else
            {
                basket.Items.Add(new BasketItemDto(
                    menuDetails.Id,
                    menuDetails.Name,
                    menuDetails.Price,
                    request.Request.Quantity,
                    menuDetails.ImageUrl
                ));
            }

            // 4. Recalculate total price
            basket = basket with { TotalPrice = basket.Items.Sum(item => item.Price * item.Quantity) };

            // 5. Store updated basket
            await repository.StoreBasketAsync(basket, cancellationToken);

            logger.LogInformation("Item {MenuName} added to basket for user {UserName}. Current total price: {TotalPrice}", menuDetails.Name, request.UserName, basket.TotalPrice);

            return basket;
        }
    }
}