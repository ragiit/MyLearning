using Microsoft.EntityFrameworkCore;

namespace MyBlazor.Repository
{
    public class OrderRepository(ApplicationDbContext context) : IOrderRepository
    {
        public async Task<OrderHeader?> GetByIdAsync(int id)
        {
            return await context.OrderHeaders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<OrderHeader> CreateAsync(OrderHeader orderHeader)
        {
            orderHeader.OrderDate = DateTime.Now;
            context.OrderHeaders.Add(orderHeader);
            await context.SaveChangesAsync();
            return orderHeader;
        }

        public async Task<IEnumerable<OrderHeader>> GetAllAsync(string userId)
        {
            return await context.OrderHeaders.Where(o => o.ApplicationUserId == userId).ToListAsync();
        }

        public async Task<OrderHeader> UpdateStatusAsync(int orderId, string status)
        {
            var order = await context.OrderHeaders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await context.SaveChangesAsync();
            }
            return order;
        }

        public async Task<IEnumerable<OrderHeader>> GetAllOrdersAsync(string? userId = null)
        {
            IQueryable<OrderHeader> query = context.OrderHeaders
                                                    .Include(oh => oh.ApplicationUser) // Penting untuk nama pengguna
                                                    .Include(oh => oh.OrderDetails)
                                                        .ThenInclude(od => od.Product); // Penting untuk detail produk

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(oh => oh.ApplicationUserId == userId);
            }

            return await query.OrderByDescending(oh => oh.OrderDate).ToListAsync();
        }

        public async Task<OrderHeader?> GetOrderByIdAsync(int id)
        {
            return await context.OrderHeaders
                                .Include(oh => oh.ApplicationUser)
                                .Include(oh => oh.OrderDetails)
                                    .ThenInclude(od => od.Product)
                                .FirstOrDefaultAsync(oh => oh.Id == id);
        }
    }
}