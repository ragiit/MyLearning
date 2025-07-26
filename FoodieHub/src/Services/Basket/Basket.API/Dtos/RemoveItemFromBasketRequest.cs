namespace Basket.API.Dtos
{
    public record RemoveItemFromBasketRequest(
       [Required] Guid MenuId
   );
}