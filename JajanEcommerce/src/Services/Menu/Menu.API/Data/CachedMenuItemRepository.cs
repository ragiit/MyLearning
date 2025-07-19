using BuildingBlocks.Pagination;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Menu.API.Data
{
    public class CachedMenuItemRepository(IMenuItemRepository repository, IDistributedCache cache) : IMenuItemRepository
    {
        private const string CachePrefix = "menuitem_";
        private const string AllCacheKey = $"{CachePrefix}all";

        private string ItemCacheKey(Guid id) => $"{CachePrefix}{id}";

        public async Task<List<MenuItem>> GetAllAsync(int? pageIndex = 1, int? pageSize = 10, CancellationToken cancellationToken = default)
        {
            var cached = await cache.GetStringAsync(AllCacheKey, cancellationToken);
            List<MenuItem> allItems;

            if (!string.IsNullOrWhiteSpace(cached))
            {
                allItems = JsonSerializer.Deserialize<List<MenuItem>>(cached)!;
            }
            else
            {
                allItems = await repository.GetAllAsync(cancellationToken: cancellationToken);
                await cache.SetStringAsync(AllCacheKey, JsonSerializer.Serialize(allItems), cancellationToken);
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                var skip = (pageIndex.Value - 1) * pageSize.Value;
                return allItems.Skip(skip).Take(pageSize.Value).ToList();
            }

            return allItems;
        }

        public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = ItemCacheKey(id);
            var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(cached))
                return JsonSerializer.Deserialize<MenuItem>(cached)!;

            var item = await repository.GetByIdAsync(id, cancellationToken);
            if (item is not null)
            {
                await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(item), cancellationToken);
            }

            return item;
        }

        public async Task<MenuItem?> UpdateAsync(UpdateMenuItemDto dto, CancellationToken cancellationToken = default)
        {
            var updated = await repository.UpdateAsync(dto, cancellationToken);
            if (updated is null) return null;

            var cacheKey = ItemCacheKey(updated.Id);
            await cache.RemoveAsync(cacheKey, cancellationToken);
            await cache.RemoveAsync(AllCacheKey, cancellationToken);
            return updated;
        }

        public async Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
        {
            await repository.AddAsync(menuItem, cancellationToken);
            await cache.RemoveAsync(AllCacheKey, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            // Biasanya tidak perlu cache untuk Exists
            return await repository.ExistsByNameAsync(name, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await repository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                await cache.RemoveAsync(ItemCacheKey(id), cancellationToken);
                await cache.RemoveAsync(AllCacheKey, cancellationToken);
            }
            return result;
        }
    }
}