namespace Apple.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController(AppDbContext db) : ControllerBase
    {
        private readonly ResponseDto _response = new();

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

                // Konversi ke entitas OrderHeader dan tambahkan detailnya
                var orderHeader = orderHeaderDto.Adapt<OrderHeader>();
                orderHeader.OrderDetails = cartDto.CartDetails.Adapt<List<OrderDetail>>().Select(x => new OrderDetail
                {
                    ProductName = x.Product.Name,
                    ProductId = x.ProductId,
                    Price = x.Product.Price,
                }).ToList();

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

        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                var orderHeader = await db.OrderHeaders.FindAsync(orderId);
                if (orderHeader != null)
                {
                    orderHeader.Status = newStatus;
                    await db.SaveChangesAsync();
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
    }
}