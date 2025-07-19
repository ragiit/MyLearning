using Order.Domain.Abstractions;
using Order.Domain.Events;
using Order.Domain.ValueObjects;

namespace Order.Domain.Models
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Preparing,
        Cooking,
        Delivered,
        Cancelled
    }

    public class Order : Aggregate<OrderId>
    {
        public CustomerId CustomerId { get; private set; } = default!;
        public OrderName OrderName { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public DateTime Date { get; private set; } = DateTime.UtcNow;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        private readonly List<OrderItem> _orderItems = [];
        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public decimal TotalPrice => OrderItems.Sum(x => x.Price * x.Quantity);

        private Order()
        { }

        public static Order Create(OrderId id, CustomerId customerId, OrderName orderName, Payment payment)
        {
            var order = new Order
            {
                Id = id,
                CustomerId = customerId,
                OrderName = orderName,
                Payment = payment,
                Status = OrderStatus.Pending
            };

            order.AddDomainEvent(new OrderCreatedEvent(order));
            return order;
        }

        public void Update(OrderName orderName, Payment payment, OrderStatus status)
        {
            OrderName = orderName;
            Payment = payment;
            Status = status;

            AddDomainEvent(new OrderUpdatedEvent(this));
        }

        public void AddItem(MenuItemId menuItemId, string productName, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var item = new OrderItem(Id, menuItemId, productName, quantity, price);
            _orderItems.Add(item);
        }

        public void RemoveItem(MenuItemId menuItemId)
        {
            var item = _orderItems.FirstOrDefault(x => x.MenuItemId == menuItemId);
            if (item is not null)
            {
                _orderItems.Remove(item);
            }
        }
    }
}