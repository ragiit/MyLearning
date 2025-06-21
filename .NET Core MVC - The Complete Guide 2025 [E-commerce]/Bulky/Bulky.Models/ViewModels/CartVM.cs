using Bulky.Models.Models;

namespace Bulky.Models.ViewModels
{
    public class CartVM
    {
        public IEnumerable<Cart> CartList { get; set; } = [];
        public OrderHeader OrderHeader { get; set; } = new();
    }
}