using System.Text.Json.Serialization;

namespace Basket.API.Dtos;

public enum PaymentMethod
{
    Cash = 0,
    BankTransfer = 1,
    QRIS = 2
}

public class BasketCheckoutDto
{
    public DateTime Date { get; set; }
    public PaymentMethod? PaymentMethod { get; set; } = Dtos.PaymentMethod.Cash;
    public decimal CashAmount { get; set; } = 0;
    public List<OrderItemDto> OrderItems { get; set; } = [];
}

public class OrderItemDto
{
    public Guid MenuItemId { get; set; }
    public int Quantity { get; set; }
}

public class BasketCheckoutRequestDto
{
    [JsonIgnore]
    public DateTime Date { get; set; }

    public PaymentMethod? PaymentMethod { get; set; } = Dtos.PaymentMethod.Cash;
    public decimal CashAmount { get; set; } = 0;
}