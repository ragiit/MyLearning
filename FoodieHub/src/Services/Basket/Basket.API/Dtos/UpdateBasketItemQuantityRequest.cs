namespace Basket.API.Dtos
{
    public record UpdateBasketItemQuantityRequest(
         [Required] Guid MenuId,
         [Required][Range(0, int.MaxValue)] int Quantity
     );
}