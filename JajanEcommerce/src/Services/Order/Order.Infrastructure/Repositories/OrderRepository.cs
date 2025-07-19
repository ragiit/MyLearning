using Microsoft.EntityFrameworkCore;
using Order.Domain.Models;
using Order.Domain.Repositories;

namespace Order.Infrastructure.Repositories
{
    public class OrderRepository(AppDbContext dbContext) : IOrderRepository
    {
        public async Task AddAsync(Order.Domain.Models.Order order, CancellationToken cancellationToken = default)
        {
            await dbContext.Orders.AddAsync(order, cancellationToken);
        }

        public async Task<Order.Domain.Models.Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id.Value == orderId, cancellationToken);
        }

        public async Task<IReadOnlyList<Order.Domain.Models.Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            var allOrders = await dbContext.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return allOrders
                .Where(o => o.CustomerId.Value == customerId)
                .ToList();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}