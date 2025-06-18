using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; } = new CategoryRepository(context);
        public IProductRepository Product { get; private set; } = new ProductRepository(context);
        public ICompanyRepository Company { get; private set; } = new CompanyRepository(context);
        public ICartRepository Cart { get; private set; } = new CartRepository(context);
        public IApplicationUserRepository ApplicationUser { get; private set; } = new ApplicationUserRepository(context);

        public void Save()
        {
            context.SaveChanges();
        }
    }
}