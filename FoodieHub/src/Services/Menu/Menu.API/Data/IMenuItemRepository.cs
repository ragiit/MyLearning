namespace Menu.API.Data
{
    public interface IMenuItemRepository
    {
        Task<List<Persistence.Entities.Menu>> GetAllAsync(int? pageIndex = null, int? pageSize = null, CancellationToken cancellationToken = default);

        Task<Persistence.Entities.Menu?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Persistence.Entities.Menu?> UpdateAsync(Menu.API.Dtos.UpdateMenuRequest dto, CancellationToken cancellationToken = default);

        Task AddAsync(Persistence.Entities.Menu menu, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}