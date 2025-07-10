namespace MyApp.Data.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        Task<Product?> GetByIdAsync(int id);

        Task<Product> AddAsync(Product product);

        Task<Product> UpdateAsync(Product product);

        Task DeleteAsync(int id);
    }
}