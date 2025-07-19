using Order.Domain.Abstractions;
using Order.Domain.ValueObjects;

namespace Order.Domain.Models
{
    public class OrderItem : Aggregate<OrderItemId>
    {
        public OrderId OrderId { get; private set; }
        public MenuItemId MenuItemId { get; private set; }
        public string MenuName { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        protected OrderItem()
        { }

        internal OrderItem(OrderId orderId, MenuItemId menuItemId, string productName, int quantity, decimal price)
        {
            Id = OrderItemId.Of(Guid.NewGuid());
            OrderId = orderId;
            MenuItemId = menuItemId;
            MenuName = productName;
            Quantity = quantity;
            Price = price;
        }
    }
}