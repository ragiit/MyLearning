using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // Memastikan hanya user yang sudah login yang bisa akses
    public class MyOrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MyOrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Action untuk menampilkan daftar pesanan milik user
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Ambil HANYA pesanan milik user yang sedang login
            var orderHeaders = _unitOfWork.OrderHeader.GetAll(
                u => u.ApplicationUserId == userId,
                includes: "ApplicationUser");

            return View(orderHeaders);
        }

        // Action untuk menampilkan detail satu pesanan
        public IActionResult Details(int orderId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId && u.ApplicationUserId == userId, includes: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includes: "Product")
            };

            // Jika pesanan tidak ditemukan ATAU bukan milik user yang login, tampilkan error
            if (orderVM.OrderHeader == null)
            {
                return NotFound();
            }

            return View(orderVM);
        }
    }
}