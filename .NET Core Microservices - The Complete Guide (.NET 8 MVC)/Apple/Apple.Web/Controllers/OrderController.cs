using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Apple.Web.Controllers
{
    public class OrderController(IOrderService orderService, ICartService cartService) : Controller
    {
        // Action untuk menampilkan daftar pesanan.
        // Admin melihat semua pesanan, user biasa hanya melihat pesanannya sendiri.
        [Authorize]
        public async Task<IActionResult> OrderIndex()
        {
            string? userId = User.IsInRole(SD.RoleAdmin) ? null :
                User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;

            var response = await orderService.GetOrdersForUserAsync(userId);
            List<OrderHeaderDto> list = new();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        // Action untuk menampilkan detail dari satu pesanan.
        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto? orderHeaderDto = null;
            var response = await orderService.GetOrderAsync(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
            }
            if (orderHeaderDto == null)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        // Action POST untuk memperbarui status pesanan (hanya untuk Admin).
        [HttpPost("UpdateOrderStatus")]
        [Authorize(Roles = SD.RoleAdmin)]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var response = await orderService.UpdateOrderStatusAsync(orderId, status);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId });
            }
            TempData["error"] = "Failed to update status";
            return RedirectToAction(nameof(OrderDetail), new { orderId });
        }

        // Action untuk halaman konfirmasi setelah pembayaran Stripe berhasil.
        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            var response = await orderService.ValidateStripeSession(orderId);
            if (response == null || !response.IsSuccess)
            {
                TempData["error"] = response?.Message ?? "Failed to validate order.";
                return RedirectToAction(nameof(OrderIndex));
            }

            var orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));

            // Jika pembayaran berhasil (status "Approved"), bersihkan keranjang belanja.
            if (orderHeader.Status == SD.StatusApproved)
            {
                //await cartService.RemoveCartAsync(orderHeader.UserId);
            }

            return View(orderId);
        }
    }
}