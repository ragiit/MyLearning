using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using System.Diagnostics;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includes: "Category");
            return View(productList);
        }

        public IActionResult Details(int? productId)
        {
            // Ambil satu produk dari database berdasarkan ID yang diterima,
            // jangan lupa sertakan data Kategori-nya.
            Product? product = _unitOfWork.Product.Get(u => u.Id == productId, includes: "Category");

            if (product == null)
            {
                // Jika produk dengan ID tersebut tidak ditemukan, tampilkan halaman Not Found.
                return NotFound();
            }

            // Kirim objek produk langsung ke View.
            return View(product);
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