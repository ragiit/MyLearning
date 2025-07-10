using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;

namespace Apple.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController(AppDbContext db) : ControllerBase
    {
        private readonly ResponseDto _response = new();

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    orderHeaders = db.OrderHeaders
                        .Include(u => u.OrderDetails)
                        .ToList();
                }
                else
                {
                    orderHeaders = db.OrderHeaders
                        .Include(u => u.OrderDetails) // Penting untuk memuat detail
                        .Where(u => u.UserId == userId)
                        .ToList();
                }
                _response.Result = orderHeaders.Adapt<IEnumerable<OrderHeaderDto>>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{orderId:int}")]
        public async Task<ResponseDto> GetOrder(int orderId)
        {
            try
            {
                var orderHeader = await db.OrderHeaders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
                if (orderHeader == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order not found";
                }
                else
                {
                    _response.Result = orderHeader.Adapt<OrderHeaderDto>();
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                var orderHeader = await db.OrderHeaders.FindAsync(orderId);
                if (orderHeader != null)
                {
                    if (newStatus == SD.StatusCancelled)
                    {
                        var option = new Stripe.RefundCreateOptions
                        {
                            PaymentIntent = orderHeader.PaymentIntentId,
                            Reason = Stripe.RefundReasons.RequestedByCustomer,
                        };

                        var service = new Stripe.RefundService();
                        var refund = await service.CreateAsync(option);
                    }

                    orderHeader.Status = newStatus;
                    await db.SaveChangesAsync();
                    _response.Result = orderHeader.Adapt<OrderHeaderDto>();
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Order not found";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                // Buat OrderHeader dari CartHeaderDto
                OrderHeaderDto orderHeaderDto = cartDto.CartHeader.Adapt<OrderHeaderDto>();
                orderHeaderDto.Id = 0;
                orderHeaderDto.Date = DateTime.Now;
                orderHeaderDto.Status = SD.StatusPending;
                orderHeaderDto.OrderTotal = cartDto.CartHeader.CartTotal;

                // Konversi ke entitas OrderHeader dan tambahkan detailnya
                var orderHeader = orderHeaderDto.Adapt<OrderHeader>();
                orderHeader.OrderDetails = [.. cartDto.CartDetails.Adapt<List<OrderDetail>>().Select(x => new OrderDetail
                {
                    ProductName = x.Product.Name,
                    ProductId = x.ProductId,
                    Price = x.Product.Price,
                    Count = x.Count,
                })];

                // Simpan ke database
                var result = await db.OrderHeaders.AddAsync(orderHeader);
                await db.SaveChangesAsync();

                _response.Result = result.Entity.Adapt<OrderHeaderDto>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("GetOrders/{userId?}")]
        public async Task<ResponseDto> GetOrders(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> orderHeaders;
                if (!string.IsNullOrEmpty(userId))
                {
                    orderHeaders = await db.OrderHeaders
                        .Include(u => u.OrderDetails) // Penting untuk memuat detail
                        .Where(u => u.UserId == userId)
                        .ToListAsync();
                }
                else
                {
                    orderHeaders = await db.OrderHeaders
                        .Include(u => u.OrderDetails)
                        .ToListAsync();
                }
                _response.Result = orderHeaders.Adapt<IEnumerable<OrderHeaderDto>>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<IActionResult> CreateStripeSession([FromBody] StripeRequestDto stripeRequest)
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequest.ApprovedUrl,
                    CancelUrl = stripeRequest.CancelledUrl,
                    LineItems = [],
                    Mode = "payment",
                    Discounts = []
                };

                var discountObj = new Stripe.Checkout.SessionDiscountOptions
                {
                    Coupon = stripeRequest.OrderHeader.CouponCode,
                };

                foreach (var item in stripeRequest.OrderHeader.OrderDetails)
                {
                    options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                            },
                            UnitAmount = (long)(item.Price * 100), // Convert to cents
                        },
                        Quantity = item.Count,
                    });
                }

                if (stripeRequest.OrderHeader.Discount > 0)
                    options.Discounts.Add(discountObj);

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);
                stripeRequest.StripeSessionUrl = session.Url;

                var oh = await db.OrderHeaders
                    .FirstOrDefaultAsync(o => o.Id == stripeRequest.OrderHeader.Id);
                oh.StripeSessionId = session.Id;
                await db.SaveChangesAsync();

                _response.Result = stripeRequest;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return Ok(_response);
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                var orderHeader = await db.OrderHeaders.FirstAsync(o => o.Id == orderHeaderId);

                var service = new SessionService();
                Session session = await service.GetAsync(orderHeader.StripeSessionId);

                var paymentIntentService = new Stripe.PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.StatusApproved;

                    await db.SaveChangesAsync();

                    var reward = new RewardDto
                    {
                        UserId = orderHeader.UserId,
                        Date = DateTime.Now,
                        RewardActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        OrderId = orderHeader.Id
                    };

                    _response.Result = orderHeader.Adapt<OrderHeaderDto>();
                }

                //// Cek status pembayaran di Stripe
                //if (session.PaymentStatus.ToLower() == "paid")
                //{
                //    // Pembayaran berhasil, perbarui status pesanan
                //    orderHeader.PaymentIntentId = session.PaymentIntentId;
                //    orderHeader.Status = "Approved";
                //    await db.SaveChangesAsync();
                //    _response.Result = orderHeader.Adapt<OrderHeaderDto>();
                //}
                //else
                //{
                //    _response.IsSuccess = false;
                //    _response.Message = "Payment was not successful.";
                //}
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}