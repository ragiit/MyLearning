using Microsoft.AspNetCore.Mvc;
using NDP.Models.ViewModels;
using NDP.Web.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using static NDP.Web.Controllers.FaqController;

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

        public async Task<IActionResult> Index()
        {
            // Mengambil data dari service (seperti di OnInitializedAsync)
            var latestProductsData = await new ProductService().GetLatestProductsAsync(1);
            var portfoliosData = await new ProductService().GetPortfoliosAsync();

            // Menyiapkan semua data ke dalam satu ViewModel
            var viewModel = new HomeViewModel
            {
                LatestProducts = latestProductsData,
                Portfolios = portfoliosData,

                // Mengatur data untuk meta tags
                MetaDescription = "Cetak custom kaos, payung, mug, souvenir, dan merchandise unik untuk segala kebutuhan Anda.",
                MetaKeywords = "custom, kaos, mug, payung, souvenir, merchandise",
                OgTitle = "New Design Print - Wujudkan Desain Impian Anda",
                OgDescription = "Cetak custom kaos, payung, mug, souvenir, dan merchandise unik.",
                OgImageUrl = "https://images.unsplash.com/photo-1527335483448-f73d29d6962f?q=80&w=2070&auto=format&fit=crop",
                OgUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}" // Mendapatkan URL saat ini
            };

            // Mengatur judul halaman
            ViewData["Title"] = "Beranda";

            // Mengirim ViewModel yang sudah terisi lengkap ke View
            return View(viewModel);
        }

        public async Task<IActionResult> HowToOrder()
        {
            ViewData["Title"] = "Pertanyaan Umum (FAQ)";
            var faqItems = await new ProductService().GetFaqItemsAsync();
            return View();
        }

        // Buat action untuk halaman lain (AboutUs, FAQ, Contact, dll)
        public IActionResult AboutUs() => View();

        public async Task<IActionResult> Faq()
        {
            ViewData["Title"] = "Pertanyaan Umum (FAQ)";
            var faqItems = await new ProductService().GetFaqItemsAsync();
            return View(faqItems);
        }

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

    public class ProductService
    {
        public Task<List<FaqItemViewModel>> GetFaqItemsAsync()
        {
            var faqItems = new List<FaqItemViewModel>
            {
                new() {
                    Id = 1,
                    Question = "Bagaimana cara memesan produk custom di New Design Print?",
                    Answer = @"<p>Anda bisa memesan dengan mudah melalui beberapa langkah:</p>
                              <ol class='list-decimal list-inside ml-4 space-y-1'>
                                <li><strong>Konsultasi:</strong> Hubungi kami via WhatsApp atau email untuk membahas kebutuhan Anda (produk, jumlah, desain).</li>
                                <li><strong>Penawaran & Desain:</strong> Kami akan memberikan penawaran. Setelah setuju, kami akan proses desain (bisa dari Anda atau kami bantu).</li>
                                <li><strong>Pembayaran:</strong> Lakukan pembayaran sesuai kesepakatan (misalnya DP).</li>
                                <li><strong>Produksi:</strong> Produk Anda akan kami proses setelah pembayaran dikonfirmasi.</li>
                                <li><strong>Pengiriman/Pengambilan:</strong> Kami akan informasikan jika produk sudah selesai dan siap dikirim atau diambil.</li>
                              </ol>
                              <p class='mt-2'>Untuk detail lebih lanjut, silakan kunjungi halaman <a href='/how-to-order' class='text-pink-600 hover:underline font-semibold'>Cara Pesan</a> kami.</p>"
                },
                new FaqItemViewModel {
                    Id = 2,
                    Question = "Berapa lama proses produksi biasanya?",
                    Answer = "<p>Lama proses produksi bervariasi tergantung jenis produk, jumlah pesanan, dan tingkat kerumitan desain. Rata-rata untuk produk seperti kaos atau mug bisa memakan waktu 3-7 hari kerja setelah desain dan pembayaran disetujui. Untuk pesanan dalam jumlah besar atau produk yang lebih kompleks, waktunya mungkin lebih lama. Kami akan selalu memberikan estimasi waktu pengerjaan saat Anda melakukan pemesanan.</p>"
                },
                // ... Tambahkan item FAQ lainnya dari kode Blazor Anda ...
            };
            return Task.FromResult(faqItems);
        }

        public Task<List<Portfolio>> GetPortfoliosAsync()
        {
            var portfolios = new List<Portfolio>
            {
                new Portfolio { Name = "Kaos Event Komunitas", Description = "Sablon DTF", ImageUrl = "https://images.unsplash.com/photo-1576566588028-4147f3842f27?w=500" },
                new Portfolio { Name = "Mug Merchandise Kantor", Description = "Cetak full color", ImageUrl = "https://images.unsplash.com/photo-1594225096333-5b8184918e7d?w=500" },
                new Portfolio { Name = "Payung Promosi Bank", Description = "Sablon 2 warna", ImageUrl = "https://images.unsplash.com/photo-1533890246415-33a2180c46b5?w=500" },
                new Portfolio { Name = "Totebag Seminar", Description = "Bahan kanvas", ImageUrl = "https://images.unsplash.com/photo-1594495893623-232b7eb0c715?w=500" },
                new Portfolio { Name = "Hoodie Angkatan", Description = "Bordir komputer", ImageUrl = "https://images.unsplash.com/photo-1556108343-6e3a6a9a0873?w=500" },
                new Portfolio { Name = "Stiker Vinyl Brand", Description = "Cutting & print", ImageUrl = "https://images.unsplash.com/photo-1579042935104-98448967484d?w=500" }
            };
            return Task.FromResult(portfolios);
        }

        public Task<List<Product>> GetLatestProductsAsync(int count)
        {
            var products = new List<Product>
            {
                new Product { Name = "Kaos Oversize Vintage", Price = 95000, Slug = "kaos-oversize-vintage", ImageUrl = "https://images.unsplash.com/photo-1622470953794-aa697e337142?w=500" },
                new Product { Name = "Tumbler Grafir Nama", Price = 150000, Slug = "tumbler-grafir-nama", ImageUrl = "https://images.unsplash.com/photo-1610393358536-a8368962551a?w=500" },
                new Product { Name = "Topi Trucker Jaring", Price = 45000, Slug = "topi-trucker-jaring", ImageUrl = "https://images.unsplash.com/photo-1588850563404-3a2a62095cad?w=500" },
                new Product { Name = "Jaket Bomber Varsity", Price = 250000, Slug = "jaket-bomber-varsity", ImageUrl = "https://images.unsplash.com/photo-1620799140408-edc6dcb6d633?w=500" },
            };
            return Task.FromResult(products);
        }
    }

    public class HomeViewModel
    {
        // Data untuk ditampilkan
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

        public List<Product> LatestProducts { get; set; } = new List<Product>();

        // Data untuk meta tag di <head>
        public string MetaDescription { get; set; } = string.Empty;

        public string MetaKeywords { get; set; } = string.Empty;
        public string OgTitle { get; set; } = string.Empty;
        public string OgDescription { get; set; } = string.Empty;
        public string OgImageUrl { get; set; } = string.Empty;
        public string OgUrl { get; set; } = string.Empty;
    }
}