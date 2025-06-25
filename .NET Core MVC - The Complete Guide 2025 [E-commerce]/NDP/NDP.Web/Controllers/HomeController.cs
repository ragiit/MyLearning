using Microsoft.AspNetCore.Mvc;
using NDP.Models.ViewModels;
using NDP.Web.Models;
using System.Diagnostics;

namespace NDP.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Anda bisa inject service Anda di sini
        // private readonly IProductService _productService;
        // public HomeController(IProductService productService) { ... }

        public IActionResult Index()
        {
            // Simulasi pengambilan data
            var latestProducts = new List<Product> { /* ... data produk ... */ };
            var portfolios = new List<Portfolio> { /* ... data portfolio ... */ };

            var viewModel = new HomeViewModel
            {
                LatestProducts = latestProducts,
                Portfolios = portfolios
            };

            ViewData["Title"] = "New Design Print | Solusi Produk Custom Berkualitas";
            return View(viewModel);
        }

        // Buat action untuk halaman lain (AboutUs, FAQ, Contact, dll)
        public IActionResult AboutUs() => View();

        public IActionResult Faq() => View();

        public IActionResult Contact() => View();

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