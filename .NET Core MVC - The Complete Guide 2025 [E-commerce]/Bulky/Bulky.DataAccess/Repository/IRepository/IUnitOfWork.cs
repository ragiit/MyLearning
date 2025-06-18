namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        ICartRepository Cart { get; }
        IApplicationUserRepository ApplicationUser { get; }

        void Save();
    }
}