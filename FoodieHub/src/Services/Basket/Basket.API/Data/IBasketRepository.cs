namespace Basket.API.Data
{
    public interface IBasketRepository
    {
        Task<BasketDto?> GetBasketAsync(string userName, CancellationToken cancellationToken = default);

        Task<BasketDto> StoreBasketAsync(BasketDto basket, CancellationToken cancellationToken = default);

        Task DeleteBasketAsync(string userName, CancellationToken cancellationToken = default);
    }
}