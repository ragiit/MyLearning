namespace Apple.Web.Service
{
    public class OrderService(IBaseService baseService) : IOrderService
    {
        public async Task<ResponseDto?> CreateOrderAsync(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = cartDto,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequest)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = stripeRequest,
                Url = SD.OrderAPIBase + "/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseDto?> GetOrdersForUserAsync(string userId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.OrderAPIBase + $"/api/order/GetOrders/{userId}"
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = newStatus, // Mengirim status baru di body request
                Url = SD.OrderAPIBase + $"/api/order/UpdateOrderStatus/{orderId}"
            });
        }

        public async Task<ResponseDto?> GetOrderAsync(int orderId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.OrderAPIBase + $"/api/order/GetOrder/{orderId}"
            });
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = orderHeaderId, // Mengirim status baru di body request
                Url = SD.OrderAPIBase + $"/api/order/ValidateStripeSession"
            });
        }
    }
}