using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Apple.Web.Controllers
{
    [Authorize]
    public class CartController(ICartService cartService, ICouponService couponService, IOrderService orderService) : Controller
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

        // --- ACTION GET UNTUK MENAMPILKAN HALAMAN CHECKOUT ---
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        // --- ACTION POST UNTUK MEMPROSES CHECKOUT ---
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            // 1. Ambil data keranjang terbaru untuk memastikan konsistensi
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            // 2. Salin detail pengiriman dari form ke objek keranjang
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            //3.Panggil service untuk membuat pesanan
            ResponseDto? response = await orderService.CreateOrderAsync(cart);
            if (response != null && response.IsSuccess)
            {
                // Pesanan berhasil dibuat, redirect ke halaman konfirmasi
                // Anda bisa membuat halaman konfirmasi atau redirect ke home
                TempData["success"] = "Order placed successfully!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(cart);
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