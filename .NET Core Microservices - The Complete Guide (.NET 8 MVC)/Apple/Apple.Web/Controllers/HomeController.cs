using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Apple.Web.Controllers
{
    public class HomeController(IProductService productService, ICartService cartService, ILogger<HomeController> logger) : Controller
    {
        // Action Index sekarang mengambil semua produk dan menampilkannya.
        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = [];
            ResponseDto? response = await productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }

        // Action baru untuk menampilkan halaman detail produk.
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? model = new();
            ResponseDto? response = await productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize] // Hanya user yang sudah login yang bisa menambahkan ke keranjang
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            // Dapatkan UserId dari user yang sedang login
            var userId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;

            CartDto cartDto = new()
            {
                CartHeader = new()
                {
                    UserId = userId
                },
                CartDetails =
                [
                    new()
                    {
                        Count = Convert.ToInt32(productDto.Count),
                        ProductId = productDto.Id
                    }
                ]
            };

            // Panggil service untuk menambahkan/memperbarui item di keranjang
            ResponseDto? response = await cartService.UspsertCartAsync(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added to the Shopping Cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            // Jika gagal, kembali ke halaman detail produk yang sama
            // Kita perlu mengambil lagi detail produknya untuk ditampilkan
            var productResponse = await productService.GetProductByIdAsync(productDto.Id);
            if (productResponse != null && productResponse.IsSuccess)
            {
                var fullProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(productResponse.Result));
                return View(fullProductDto);
            }

            return NotFound();
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