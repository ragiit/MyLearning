using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Helper.Role_Admin + "," + Helper.Role_Employee)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includes: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includes: "Product")
            };

            if (OrderVM.OrderHeader == null)
            {
                return NotFound();
            }

            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            // Update hanya field yang boleh diubah oleh admin
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Detail Pesanan berhasil diperbarui.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        public IActionResult SetToInProcess()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, Helper.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Status Pesanan diubah menjadi 'Dalam Proses'.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult SetToShipped()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = Helper.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == Helper.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["success"] = "Status Pesanan diubah menjadi 'Dikirim'.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult PayNow()
        {
            // Ambil kembali data pesanan dari DB untuk memastikan integritas
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includes: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includes: "Product");

            // --- LOGIKA MEMBUAT SESI CHECKOUT STRIPE ---
            var domain = "https://localhost:7152/"; // Ganti dengan domain aplikasi Anda saat production
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            // Tambahkan setiap item pesanan ke dalam sesi Stripe
            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // Harga dalam sen (contoh: 20.50 -> 2050)
                        Currency = "usd", // Ganti ke "idr" jika akun Stripe Anda mendukung Rupiah
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            // Simpan SessionId dan PaymentIntentId dari Stripe ke database kita
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            // Arahkan browser ke halaman pembayaran Stripe
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == Helper.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, Helper.StatusApproved, Helper.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }

            // Nanti bisa ditambahkan logika untuk customer biasa
            // ...

            return View(orderHeaderId);
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null)
            {
                TempData["Error"] = "Pesanan tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }

            // Cek apakah pesanan sudah dibayar atau belum
            if (orderHeader.PaymentStatus == Helper.PaymentStatusApproved)
            {
                // --- LOGIKA REFUND DENGAN STRIPE ---
                // Jika pembayaran sudah berhasil (Approved), kita perlu melakukan refund melalui Stripe

                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer, // Alasan refund
                    PaymentIntent = orderHeader.PaymentIntentId // ID transaksi yang akan di-refund
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                // Update status di database kita SETELAH proses refund di Stripe berhasil
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Helper.StatusCancelled, Helper.StatusRefunded);
            }
            else
            {
                // Jika pesanan belum dibayar atau statusnya masih pending, cukup batalkan statusnya di DB kita
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, Helper.StatusCancelled, Helper.StatusCancelled);
            }

            _unitOfWork.Save();
            TempData["Success"] = "Pesanan berhasil dibatalkan.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string? status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(Helper.Role_Admin) || User.IsInRole(Helper.Role_Employee))
            {
                // Jika ya, ambil semua pesanan
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includes: "ApplicationUser").ToList();
            }
            else
            {
                // Jika bukan, dia adalah customer. Ambil ID user yang sedang login.
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Ambil hanya pesanan milik user tersebut
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includes: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == Helper.PaymentStatusDelayedPayment || u.OrderStatus == Helper.StatusPending);
                    break;

                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Helper.StatusInProcess);
                    break;

                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Helper.StatusShipped);
                    break;

                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == Helper.StatusApproved);
                    break;

                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        #endregion API CALLS
    }
}