using Menu.API.Data;

namespace Menu.API.Handler
{
    public sealed record GetAllMenuItemsQuery(int? PageIndex = null, int? PageSize = null) : IQuery<List<MenuItemDto>>;

    public sealed class GetAllMenuItemsHandler(IMenuItemRepository repo) : IQueryHandler<GetAllMenuItemsQuery, List<MenuItemDto>>
    {
        public async Task<List<MenuItemDto>> Handle(GetAllMenuItemsQuery request, CancellationToken cancellationToken)
        {
            var menus = await repo.GetAllAsync(request.PageIndex, request.PageSize, cancellationToken);
            return menus.Adapt<List<MenuItemDto>>();
        }
    }
}