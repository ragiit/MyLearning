namespace MyBlazor.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<OrderHeader?> GetByIdAsync(int id);

        Task<IEnumerable<OrderHeader>> GetAllOrdersAsync(string? userId = null);

        Task<OrderHeader?> GetOrderByIdAsync(int id);

        Task<OrderHeader> CreateAsync(OrderHeader orderHeader);

        Task<IEnumerable<OrderHeader>> GetAllAsync(string userId);

        Task<OrderHeader> UpdateStatusAsync(int orderId, string status);
    }
}