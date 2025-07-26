// Menu.API/Data/CachedMenuItemRepository.cs
using BuildingBlocks.Pagination; // Pastikan ini di-import jika digunakan di metode GetAllAsync
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Menu.API.Data
{
    // Menggunakan entitas Menu, bukan MenuItem generik
    public class CachedMenuItemRepository(IMenuItemRepository repository, IDistributedCache cache) : IMenuItemRepository
    {
        private const string CachePrefix = "menuitem_";
        private const string AllCacheKey = $"{CachePrefix}all"; // Cache untuk semua item

        private string ItemCacheKey(Guid id) => $"{CachePrefix}{id}";

        public async Task<List<Persistence.Entities.Menu>> GetAllAsync(int? pageIndex = 1, int? pageSize = 10, CancellationToken cancellationToken = default)
        {
            var cached = await cache.GetStringAsync(AllCacheKey, cancellationToken);
            List<Persistence.Entities.Menu> allItems;

            if (!string.IsNullOrWhiteSpace(cached))
            {
                allItems = JsonSerializer.Deserialize<List<Persistence.Entities.Menu>>(cached)!;
            }
            else
            {
                // Panggil repository sebenarnya tanpa paging, lalu cache semua
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

        public async Task<Persistence.Entities.Menu?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = ItemCacheKey(id);
            var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrWhiteSpace(cached))
                return JsonSerializer.Deserialize<Persistence.Entities.Menu>(cached)!;

            var item = await repository.GetByIdAsync(id, cancellationToken);
            if (item is not null)
            {
                await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(item), cancellationToken);
            }

            return item;
        }

        public async Task<Persistence.Entities.Menu?> UpdateAsync(UpdateMenuRequest dto, CancellationToken cancellationToken = default)
        {
            var updated = await repository.UpdateAsync(dto, cancellationToken);
            if (updated is null) return null;

            var cacheKey = ItemCacheKey(updated.Id);
            await cache.RemoveAsync(cacheKey, cancellationToken);
            await cache.RemoveAsync(AllCacheKey, cancellationToken); // Invalidate all cache on update
            return updated;
        }

        public async Task AddAsync(Persistence.Entities.Menu menuItem, CancellationToken cancellationToken = default)
        {
            await repository.AddAsync(menuItem, cancellationToken);
            await cache.RemoveAsync(AllCacheKey, cancellationToken); // Invalidate all cache on add
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await repository.ExistsByNameAsync(name, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await repository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                await cache.RemoveAsync(ItemCacheKey(id), cancellationToken);
                await cache.RemoveAsync(AllCacheKey, cancellationToken); // Invalidate all cache on delete
            }
            return result;
        }
    }
}