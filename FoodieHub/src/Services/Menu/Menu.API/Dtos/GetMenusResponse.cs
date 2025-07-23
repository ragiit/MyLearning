// Menu.API/Dtos/GetMenusResponse.cs
namespace Menu.API.Dtos
{
    public record GetMenusResponse(
        int PageNumber,
        int PageSize,
        int TotalCount,
        IEnumerable<MenuDto> Menus
    );
}