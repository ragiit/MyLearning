using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; } = new CategoryRepository(context);
        public IProductRepository Product { get; private set; } = new ProductRepository(context);

        public void Save()
        {
            context.SaveChanges();
        }
    }
}