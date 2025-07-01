using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Asumsi Anda punya service untuk mengambil data keranjang.
// Jika tidak ada, Anda bisa melakukan simulasi data di sini.
// using NewDesignPrint.Services;

namespace NDP.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        // Gantikan ini dengan inject service keranjang Anda yang sesungguhnya.
        // private readonly ICartService _cartService;
        // public HeaderViewComponent(ICartService cartService)
        // {
        //    _cartService = cartService;
        // }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // --- SIMULASI PENGAMBILAN DATA KERANJANG ---
            // Di aplikasi nyata, Anda akan mengambil ini dari Session atau service
            var cartItems = new List<CartItemViewModel>
            {
                new CartItemViewModel {
                    ProductId = 101,
                    ProductName = "Kaos Polos Keren",
                    Quantity = 2,
                    Price = 75000,
                    ImageUrl = "https://placehold.co/80x80/E2E8F0/A0AEC0?text=Kaos"
                },
                new CartItemViewModel {
                    ProductId = 102,
                    ProductName = "Payung Custom Logo",
                    Quantity = 1,
                    Price = 120000,
                    ImageUrl = "https://placehold.co/80x80/E2E8F0/A0AEC0?text=Payung"
                }
            };
            // --- AKHIR SIMULASI ---

            var viewModel = new HeaderViewModel
            {
                CartItems = cartItems,
                TotalItems = cartItems.Sum(item => item.Quantity),
                TotalAmount = cartItems.Sum(item => item.SubTotal)
            };

            // ViewComponent akan merender Partial View yang ada di
            // /Views/Shared/Components/Header/Default.cshtml
            return View(viewModel);
        }
    }

    // --- Letakkan Model ini di folder /Models ---
    // File: /Models/HeaderViewModel.cs
    public class HeaderViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // File: /Models/CartItemViewModel.cs
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public decimal SubTotal => Quantity * Price;
    }
}