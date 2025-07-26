namespace Basket.API.Dtos
{
    public record AddItemToBasketRequest(
         [Required] Guid MenuId,
         [Required][Range(1, int.MaxValue)] int Quantity
     );
}