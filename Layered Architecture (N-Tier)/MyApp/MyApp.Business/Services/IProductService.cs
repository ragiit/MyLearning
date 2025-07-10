namespace MyApp.Business.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        Task<Product?> GetByIdAsync(int id);

        Task<Product> CreateAsync(Product product);

        Task<Product> UpdateAsync(Product product);

        Task DeleteAsync(int id);
    }
}