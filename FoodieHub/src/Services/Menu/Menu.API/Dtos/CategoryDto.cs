// Menu.API/Dtos/CategoryDto.cs
namespace Menu.API.Dtos
{
    public record CategoryDto(
        Guid Id,
        string Name,
        string? Description,
        string? ImageUrl,
        bool IsActive
    );
}