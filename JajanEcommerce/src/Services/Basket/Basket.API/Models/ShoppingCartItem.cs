using System.Text.Json.Serialization;

namespace Basket.API.Models
{
    public class ShoppingCartItem
    {
        public Guid MenuItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Price * Quantity;
    }

    public class ShoppingCartItemDto
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}