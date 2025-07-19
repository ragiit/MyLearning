using Order.Domain.Models;

namespace Order.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Models.Order order, CancellationToken cancellationToken = default);

        Task<Models.Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Models.Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}