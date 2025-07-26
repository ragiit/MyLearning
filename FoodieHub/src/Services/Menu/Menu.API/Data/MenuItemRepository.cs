namespace Menu.API.Data
{
    public class MenuItemRepository(ApplicationDbContext db) : IMenuItemRepository
    {
        public async Task<List<Persistence.Entities.Menu>> GetAllAsync(int? pageIndex = null, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            var query = db.Menus.AsNoTracking();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Persistence.Entities.Menu?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await db.Menus.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<Persistence.Entities.Menu?> UpdateAsync(UpdateMenuRequest dto, CancellationToken cancellationToken = default)
        {
            var menu = await db.Menus.FirstOrDefaultAsync(m => m.Id == dto.Id, cancellationToken);
            if (menu is null) return null;

            // Manual mapping atau Mapster.Adapt, sesuaikan dengan logika UpdateMenuHandler
            menu.Name = dto.Name;
            menu.Description = dto.Description;
            menu.Price = dto.Price;
            menu.CategoryId = dto.CategoryId;
            menu.IsAvailable = dto.IsAvailable;
            // ImageUrl akan di-handle di handler

            db.Menus.Update(menu);
            await db.SaveChangesAsync(cancellationToken);
            return menu;
        }

        public async Task AddAsync(Persistence.Entities.Menu menuItem, CancellationToken cancellationToken = default)
        {
            db.Menus.Add(menuItem);
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await db.Menus.AnyAsync(m => m.Name == name, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var menu = await db.Menus.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
            if (menu is null) return false;

            db.Menus.Remove(menu);
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}