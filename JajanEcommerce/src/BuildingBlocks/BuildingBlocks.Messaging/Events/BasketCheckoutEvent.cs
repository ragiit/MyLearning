namespace BuildingBlocks.Messaging.Events;

public record BasketCheckoutEvent : IntegrationEvent
{
    public string Username { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int PaymentMethod { get; set; }
    public decimal CashAmount { get; set; } = 0;
    public List<OrderItemDto> Items { get; set; } = [];
}

public class OrderItemDto
{
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}