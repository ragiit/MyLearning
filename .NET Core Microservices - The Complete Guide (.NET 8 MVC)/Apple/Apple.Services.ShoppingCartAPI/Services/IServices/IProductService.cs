namespace Apple.Services.ShoppingCartAPI.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
}