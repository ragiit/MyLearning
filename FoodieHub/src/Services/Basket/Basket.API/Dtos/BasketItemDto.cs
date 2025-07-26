namespace Basket.API.Dtos
{
    public record BasketItemDto(
        Guid MenuId,
        string MenuName,
        decimal Price,
        int Quantity,
        string? ImageUrl
    );
}