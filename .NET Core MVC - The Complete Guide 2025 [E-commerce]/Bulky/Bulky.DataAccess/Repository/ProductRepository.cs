using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository(ApplicationDbContext context) : Repository<Product>(context), IProductRepository
    {
        public void Update(Product a)
        {
            context.Products.Update(a);
        }
    }
}