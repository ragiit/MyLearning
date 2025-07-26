using Basket.API.Features.AddItemToBasket;
using Basket.API.Features.ClearBasket;
using Basket.API.Features.GetBasket;
using Basket.API.Features.RemoveItemFromBasket;
using Basket.API.Features.UpdateBasketItemQuantity; // Untuk mendapatkan User ID
using System.Security.Claims;

namespace Basket.API.Controllers
{
    /// <summary>
    /// Controller untuk operasi keranjang belanja pengguna.
    /// </summary>
    [Route("api/basket")]
    [ApiController]
    [Produces("application/json")]
    [Authorize] // Semua endpoint memerlukan otentikasi
    [EnableRateLimiting("fixed")]
    public class BasketController(ISender sender) : ControllerBase
    {
        private string UserName => User.FindFirst(ClaimTypes.NameIdentifier)?.Value // Asumsi User ID ada di NameIdentifier
                                  ?? throw new UnauthorizedAccessException("User ID claim not found.");

        /// <summary>
        /// Mendapatkan keranjang belanja pengguna saat ini.
        /// </summary>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Keranjang belanja pengguna.</returns>
        /// <response code="200">Keranjang belanja berhasil diambil.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Keranjang belanja tidak ditemukan untuk pengguna ini.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<BasketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<BasketDto>>> GetBasket(CancellationToken cancellationToken)
        {
            var query = new GetBasketQuery(UserName);
            var result = await sender.Send(query, cancellationToken);
            return Ok(new BaseResponse<BasketDto>(true, "Basket retrieved successfully", result));
        }

        /// <summary>
        /// Menambahkan item baru ke keranjang belanja pengguna.
        /// </summary>
        /// <param name="request">Detail item yang akan ditambahkan.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Keranjang belanja yang telah diperbarui.</returns>
        /// <response code="200">Item berhasil ditambahkan/diperbarui di keranjang.</response>
        /// <response code="400">Request tidak valid atau item menu tidak valid.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Item menu tidak ditemukan.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpPost("items")]
        [ProducesResponseType(typeof(BaseResponse<BasketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<BasketDto>>> AddItemToBasket([FromBody] AddItemToBasketRequest request, CancellationToken cancellationToken)
        {
            var command = new AddItemToBasketCommand(request, UserName); // Tambahkan UserName dari token
            var result = await sender.Send(command, cancellationToken);
            return Ok(new BaseResponse<BasketDto>(true, "Item added to basket successfully", result));
        }

        /// <summary>
        /// Memperbarui kuantitas item di keranjang belanja pengguna.
        /// </summary>
        /// <param name="request">Detail item dan kuantitas baru.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Keranjang belanja yang telah diperbarui.</returns>
        /// <response code="200">Kuantitas item berhasil diperbarui.</response>
        /// <response code="400">Request tidak valid atau kuantitas 0 tanpa penghapusan.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Keranjang atau item tidak ditemukan.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpPut("items")]
        [ProducesResponseType(typeof(BaseResponse<BasketDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<BasketDto>>> UpdateBasketItemQuantity([FromBody] UpdateBasketItemQuantityRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateBasketItemQuantityCommand(request, UserName);
            var result = await sender.Send(command, cancellationToken);
            return Ok(new BaseResponse<BasketDto>(true, "Item quantity updated successfully", result));
        }

        /// <summary>
        /// Menghapus item dari keranjang belanja pengguna.
        /// </summary>
        /// <param name="menuId">ID unik dari menu yang akan dihapus dari keranjang.</param>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Keranjang belanja yang telah diperbarui.</returns>
        /// <response code="200">Item berhasil dihapus dari keranjang.</response>
        /// <response code="400">Request tidak valid.</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Keranjang atau item tidak ditemukan.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpDelete("items/{menuId}")]
        [ProducesResponseType(typeof(BaseResponse<BasketDto>), StatusCodes.Status200OK)] // Biasanya kembali 200 OK dengan keranjang yang diupdate
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<BasketDto>>> RemoveItemFromBasket(Guid menuId, CancellationToken cancellationToken)
        {
            var command = new RemoveItemFromBasketCommand(new RemoveItemFromBasketRequest(menuId), UserName); // Kirim username dan menuId
            var result = await sender.Send(command, cancellationToken);
            return Ok(new BaseResponse<BasketDto>(true, "Item removed from basket successfully", result));
        }

        /// <summary>
        /// Mengosongkan seluruh keranjang belanja pengguna.
        /// </summary>
        /// <param name="cancellationToken">Token pembatalan operasi.</param>
        /// <returns>Respons kosong jika berhasil.</returns>
        /// <response code="204">Keranjang berhasil dikosongkan (No Content).</response>
        /// <response code="401">Akses tidak sah.</response>
        /// <response code="404">Keranjang tidak ditemukan.</response>
        /// <response code="429">Terlalu banyak permintaan.</response>
        /// <response code="500">Terjadi kesalahan internal server.</response>
        [HttpDelete] // DELETE /api/basket
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ClearBasket(CancellationToken cancellationToken)
        {
            var command = new ClearBasketCommand(UserName);
            await sender.Send(command, cancellationToken);
            return NoContent();
        }
    }
}