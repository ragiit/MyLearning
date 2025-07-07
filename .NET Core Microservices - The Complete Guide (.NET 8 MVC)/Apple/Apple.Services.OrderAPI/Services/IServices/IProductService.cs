using Apple.Services.OrderAPI.Models.Dto;

namespace Apple.Services.OrderAPI.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
}