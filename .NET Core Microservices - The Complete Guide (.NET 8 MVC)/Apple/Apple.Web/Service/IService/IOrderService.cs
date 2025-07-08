namespace Apple.Web.Service.IService
{
    public interface IOrderService
    {
        // Metode untuk membuat pesanan baru dari keranjang.
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);

        // Metode untuk mendapatkan semua pesanan untuk user yang sedang login.
        Task<ResponseDto?> GetOrdersForUserAsync(string userId);

        // Metode untuk memperbarui status pesanan (untuk admin).
        Task<ResponseDto?> UpdateOrderStatusAsync(int orderId, string newStatus);

        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequest);

        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
    }
}