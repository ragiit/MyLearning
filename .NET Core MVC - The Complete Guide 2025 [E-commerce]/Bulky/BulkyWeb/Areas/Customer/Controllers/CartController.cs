using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize] // Memastikan hanya user yang sudah login yang bisa mengakses keranjang
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // Gunakan [BindProperty] agar kita tidak perlu meneruskannya sebagai parameter di action POST
        [BindProperty]
        public CartVM CartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            // Ambil ID user yang sedang login
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Inisialisasi ViewModel
            CartVM = new()
            {
                // Ambil semua item di keranjang milik user, sertakan juga detail produknya
                CartList = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == userId, includes: "Product")
            };

            // Hitung total harga pesanan
            foreach (var cartItem in CartVM.CartList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem);
                CartVM.OrderTotal += (cartItem.Price * cartItem.Count);
            }

            return View(CartVM);
        }

        // Action untuk tombol (+)
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.Cart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.Cart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // Action untuk tombol (-)
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.Cart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                // Jika jumlah 1 atau kurang, hapus item dari keranjang
                _unitOfWork.Cart.Delete(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.Cart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // Action untuk tombol Hapus
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.Cart.Get(u => u.Id == cartId);
            _unitOfWork.Cart.Delete(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // Fungsi helper untuk menentukan harga berdasarkan jumlah
        private double GetPriceBasedOnQuantity(Cart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else if (cart.Count <= 100)
            {
                return cart.Product.Price50;
            }
            else
            {
                return cart.Product.Price100;
            }
        }
    }
}