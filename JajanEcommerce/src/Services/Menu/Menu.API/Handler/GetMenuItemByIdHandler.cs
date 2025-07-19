using Menu.API.Data;

namespace Menu.API.Handler
{
    public sealed record GetMenuItemByIdQuery(Guid Id) : IQuery<MenuItemDto?>;

    public sealed class GetMenuItemByIdHandler(IMenuItemRepository repo)
      : IQueryHandler<GetMenuItemByIdQuery, MenuItemDto?>
    {
        public async Task<MenuItemDto?> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
        {
            var menu = await repo.GetByIdAsync(request.Id, cancellationToken);
            return menu?.Adapt<MenuItemDto>();
        }
    }
}