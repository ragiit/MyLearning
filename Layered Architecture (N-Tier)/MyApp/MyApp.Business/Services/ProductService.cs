namespace MyApp.Business.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        => repository.GetPagedAsync(pageNumber, pageSize);

    public Task<IEnumerable<Product>> GetAllAsync()
        => repository.GetAllAsync();

    public Task<Product?> GetByIdAsync(int id)
        => repository.GetByIdAsync(id);

    public Task<Product> CreateAsync(Product product)
        => repository.AddAsync(product);

    public Task<Product> UpdateAsync(Product product)
        => repository.UpdateAsync(product);

    public Task DeleteAsync(int id)
        => repository.DeleteAsync(id);
}