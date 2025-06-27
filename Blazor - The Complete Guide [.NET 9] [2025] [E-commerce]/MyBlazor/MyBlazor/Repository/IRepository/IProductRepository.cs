namespace MyBlazor.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product?> GetByIdAsync(int id);

        Task<Product> CreateAsync(Product Product);

        Task<Product> UpdateAsync(Product Product);

        Task<bool> DeleteAsync(int id);
    }
}