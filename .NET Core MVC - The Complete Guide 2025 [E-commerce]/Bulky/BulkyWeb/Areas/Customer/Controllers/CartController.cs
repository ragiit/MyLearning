using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            CartVM = new()
            {
                CartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId, includes: "Product"),
                OrderHeader = new() // Hanya untuk menampung OrderTotal di halaman keranjang
            };

            foreach (var cartItem in CartVM.CartList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem);
                CartVM.OrderHeader.OrderTotal += (cartItem.Price * cartItem.Count);
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

        // Di dalam kelas CartController

        // === ACTION UNTUK MEMPROSES PESANAN (POST) ===
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            CartVM = new()
            {
                CartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId, includes: "Product"),
                OrderHeader = new()
            };

            // Mengisi OrderHeader dengan data user sebagai nilai default untuk form
            var appUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            CartVM.OrderHeader.Name = appUser.Name;
            CartVM.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            CartVM.OrderHeader.ApplicationUserId = userId;
            CartVM.OrderHeader.ApplicationUser = appUser;
            CartVM.OrderHeader.StreetAddress = appUser.StreetAddress;
            CartVM.OrderHeader.City = appUser.City;
            CartVM.OrderHeader.State = appUser.State;
            CartVM.OrderHeader.PostalCode = appUser.PostalCode;

            // Hitung total harga pesanan
            foreach (var cartItem in CartVM.CartList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem);
                CartVM.OrderHeader.OrderTotal += (cartItem.Price * cartItem.Count);
            }
            return View(CartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Ambil kembali CartList dari DB. `CartVM.OrderHeader` sudah terisi dari form oleh [BindProperty].
            CartVM.CartList = _unitOfWork.Cart.GetAll(u => u.ApplicationUserId == userId, includes: "Product");
            var appUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            CartVM.OrderHeader.OrderDate = DateTime.Now;
            CartVM.OrderHeader.ApplicationUserId = userId; // ID User diambil dari server, bukan dari form
            CartVM.OrderHeader.OrderStatus = Helper.StatusPending;
            CartVM.OrderHeader.PaymentStatus = Helper.PaymentStatusPending;

            // Hitung ulang totalnya di sisi server untuk keamanan dan jika validasi gagal.
            CartVM.OrderHeader.OrderTotal = 0;
            foreach (var cartItem in CartVM.CartList)
            {
                cartItem.Price = GetPriceBasedOnQuantity(cartItem);
                CartVM.OrderHeader.OrderTotal += (cartItem.Price * cartItem.Count);
            }

            // `ModelState` sekarang akan fokus memvalidasi properti di dalam `OrderHeader` (Name, City, dll.)
            if (ModelState.IsValid)
            {
                _unitOfWork.OrderHeader.Add(CartVM.OrderHeader);
                _unitOfWork.Save();

                // Buat OrderDetail
                foreach (var cartItem in CartVM.CartList)
                {
                    OrderDetail orderDetail = new()
                    {
                        ProductId = cartItem.ProductId,
                        OrderHeaderId = CartVM.OrderHeader.Id,
                        Price = cartItem.Price,
                        Count = cartItem.Count
                    };
                    _unitOfWork.OrderDetail.Add(orderDetail);
                    _unitOfWork.Save();
                }

                _unitOfWork.Cart.DeleteAll(CartVM.CartList);
                _unitOfWork.Save();

                if (appUser.CompanyId.GetValueOrDefault() == 0)
                {
                    //StripeConfiguration.ApiKey = "sk_test_51RcULpGaDDPhoP2OJKfdFP94GDWD4HLauMFut2bC1nI6frYrr4Gx61jeATCusRhrmgSEnmchKlJt2ZzRascGSdGp00zR0JG5Nh";
                    var domain = "https://localhost:7152/";
                    var options = new Stripe.Checkout.SessionCreateOptions
                    {
                        SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={CartVM.OrderHeader.Id}",
                        CancelUrl = domain + "customer/cart/index",
                        LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                    foreach (var cartItem in CartVM.CartList)
                    {
                        options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions
                        {
                            PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = cartItem.Product.Title,
                                    Description = cartItem.Product.Description,
                                },
                                UnitAmount = (long)(cartItem.Price * 100),
                            },
                            Quantity = cartItem.Count,
                        });
                    }

                    var service = new Stripe.Checkout.SessionService();
                    Stripe.Checkout.Session session = service.Create(options);
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(CartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }

                TempData["success"] = "Order placed successfully!";

                return RedirectToAction(nameof(OrderConfirmation), new { id = CartVM.OrderHeader.Id });
            }

            // Jika ModelState tidak valid, CartVM sudah lengkap (berisi CartList dan OrderHeader)
            // untuk ditampilkan kembali di View.
            return View(CartVM);
        }

        public IActionResult OrderConfirmation(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includes: "ApplicationUser");
            if (orderHeader == null)
            {
                return NotFound(); // Jika tidak ditemukan, tampilkan halaman Not Found
            }

            if (orderHeader.PaymentStatus != Helper.PaymentStatusDelayedPayment)
            {
                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, Helper.StatusApproved, Helper.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                else
                {
                    _unitOfWork.OrderHeader.UpdateStatus(id, Helper.StatusCancelled, Helper.PaymentStatusCancelled);
                    _unitOfWork.Save();
                    return View("CancelOrder"); // Tampilkan halaman pembatalan jika pembayaran tidak berhasil
                }
            }

            // Buat view untuk halaman ini
            ViewBag.OrderId = id;
            return View();
        }

        // Fungsi helper untuk menentukan harga berdasarkan jumlah
        private static double GetPriceBasedOnQuantity(Cart cart)
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