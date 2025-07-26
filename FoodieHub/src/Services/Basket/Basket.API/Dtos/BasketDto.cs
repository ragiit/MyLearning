namespace Basket.API.Dtos
{
    public record BasketDto(
          string UserName, // User ID dari token JWT
          List<BasketItemDto> Items,
          decimal TotalPrice
      );
}