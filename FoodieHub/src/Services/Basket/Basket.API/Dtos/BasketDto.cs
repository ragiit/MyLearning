namespace Basket.API.Dtos
{
    public record BasketDto(
       string UserName,
       List<BasketItemDto> Items,
       decimal TotalPrice
   )
    {
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}