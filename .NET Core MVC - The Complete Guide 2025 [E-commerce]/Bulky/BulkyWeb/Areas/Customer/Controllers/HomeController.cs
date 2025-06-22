using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            var claim = (ClaimsIdentity)User.Identity;

            if (claim.IsAuthenticated && claim is not null)
            {
                // Jika user sudah login, ambil jumlah cart dari database
                HttpContext.Session.SetInt32(Helper.SessionCart, _unitOfWork.Cart.GetAll(u => u.ApplicationUserId ==
             claim.FindFirst(ClaimTypes.NameIdentifier).Value).Count());
            }
            else if (claim == null)
            {
                // Jika user belum login, pastikan session cart dihapus
                HttpContext.Session.Clear();
            }

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includes: "Category");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            // Ambil satu produk dari database berdasarkan ID yang diterima,
            // jangan lupa sertakan data Kategori-nya.
            Cart? product = new Cart
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includes: "Category"),
                Count = 1,
                ProductId = productId
            };

            if (product == null)
            {
                // Jika produk dengan ID tersebut tidak ditemukan, tampilkan halaman Not Found.
                return NotFound();
            }

            // Kirim objek produk langsung ke View.
            return View(product);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            var scart = _unitOfWork.Cart.Get(x => x.ApplicationUserId == userId && x.ProductId == cart.ProductId);
            if (scart == null)
            {
                _unitOfWork.Cart.Add(cart);
            }
            else
            {
                scart.Count += cart.Count;
                _unitOfWork.Cart.Update(scart);
            }
            _unitOfWork.Save();

            HttpContext.Session.SetInt32(Helper.SessionCart, _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId).Count());
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}