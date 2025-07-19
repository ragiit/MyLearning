using BuildingBlocks.Pagination;

namespace Menu.API.Data
{
    public class MenuItemRepository(AppDbContext db) : IMenuItemRepository
    {
        public async Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default)
        {
            db.MenuItems.Add(menuItem);
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var menu = await db.MenuItems.FindAsync([id], cancellationToken);
            if (menu is null) return false;

            db.MenuItems.Remove(menu);
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<MenuItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await db.MenuItems.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await db.MenuItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<MenuItem?> UpdateAsync(UpdateMenuItemDto dto, CancellationToken cancellationToken = default)
        {
            var menu = await db.MenuItems.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
            if (menu is null) return null;

            dto.Adapt(menu);
            await db.SaveChangesAsync(cancellationToken);
            return menu;
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await db.MenuItems.AnyAsync(m => m.Name == name, cancellationToken);
        }

        public async Task<List<MenuItem>> GetAllAsync(int? pageIndex = null, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            var query = db.MenuItems.AsNoTracking();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                return await query
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToListAsync(cancellationToken);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}