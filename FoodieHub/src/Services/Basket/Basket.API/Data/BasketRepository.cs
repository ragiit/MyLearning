using StackExchange.Redis;
using System.Text.Json;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Basket.API.Data
{
    public class BasketRepository(IConnectionMultiplexer redis) : IBasketRepository
    {
        private readonly IDatabase _database = redis.GetDatabase();

        public async Task<BasketDto?> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            var data = await _database.StringGetAsync(userName);
            var basket = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<BasketDto>(data!);
            if (basket != null)
            {
                basket.LastUpdated = DateTime.UtcNow;
                await _database.StringSetAsync(basket.UserName, JsonSerializer.Serialize(basket));
            }
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<BasketDto>(data!);
        }

        public async Task<BasketDto> StoreBasketAsync(BasketDto basket, CancellationToken cancellationToken = default)
        {
            basket.LastUpdated = DateTime.UtcNow;
            await _database.StringSetAsync(basket.UserName, JsonSerializer.Serialize(basket));
            return await GetBasketAsync(basket.UserName, cancellationToken) ?? basket; // Mengembalikan data yang tersimpan
        }

        public async Task DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
        {
            await _database.KeyDeleteAsync(userName);
        }
    }
}