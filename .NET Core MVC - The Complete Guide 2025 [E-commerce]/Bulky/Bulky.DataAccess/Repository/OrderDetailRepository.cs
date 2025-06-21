using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class OrderDetailRepository(ApplicationDbContext context) : Repository<OrderDetail>(context), IOrderDetailRepository
    {
        public void Update(OrderDetail orderDetail)
        {
            context.OrderDetails.Update(orderDetail);
        }
    }
}