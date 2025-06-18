using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class CartRepository(ApplicationDbContext context) : Repository<Cart>(context), ICartRepository
    {
        public void Update(Cart Cart)
        {
            context.Carts.Update(Cart);
        }
    }
}