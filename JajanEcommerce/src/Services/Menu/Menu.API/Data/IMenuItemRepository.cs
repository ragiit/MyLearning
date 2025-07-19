using BuildingBlocks.Pagination;

namespace Menu.API.Data
{
    public interface IMenuItemRepository
    {
        Task<List<MenuItem>> GetAllAsync(int? pageIndex = null, int? pageSize = null, CancellationToken cancellationToken = default);

        Task<MenuItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(MenuItem menuItem, CancellationToken cancellationToken = default);

        Task<MenuItem?> UpdateAsync(UpdateMenuItemDto dto, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}