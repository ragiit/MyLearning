using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Apple.Web.Controllers
{
    [Authorize]  
    public class CartController(ICartService cartService, ICouponService couponService) : Controller
    {
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var c = await couponService.GetCouponAsync(cartDto.CartHeader.CouponCode);

            if (c != null && c.IsSuccess)
            {
                TempData["success"] = "Coupon applied successfully";
                var response = await cartService.ApplyCouponAsync(cartDto);

            }
            else
            {
                TempData["error"] = c?.Message;
            }

            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            // Set coupon code menjadi string kosong untuk dihapus di API
            cartDto.CartHeader.CouponCode = "";
            await cartService.RemoveCouponAsync(cartDto);
            return RedirectToAction(nameof(CartIndex));
        }

        public async Task<IActionResult> Remove(int cartDetailId)
        {
            await cartService.RemoveCartAsync(cartDetailId);
            return RedirectToAction(nameof(CartIndex));
        }

        // Metode helper untuk mengambil data keranjang user yang sedang login.
        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;
            ResponseDto? response = await cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            return new CartDto();
        }
    }
}