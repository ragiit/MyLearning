using BuildingBlocks;
using Refit;

namespace Basket.API.Services
{
    public interface IMenuApi
    {
        [Get("/api/menus/{id}")]
        Task<ResponseDto<MenuItemDto>> GetMenuByIdAsync(Guid id);
    }

    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string> Categories { get; set; } = new();
        public decimal Carbo { get; set; }
        public decimal Protein { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}