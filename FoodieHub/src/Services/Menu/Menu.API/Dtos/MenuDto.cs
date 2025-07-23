namespace Menu.API.Dtos
{
    public record MenuDto(
        Guid Id,
        string Name,
        string? Description,
        decimal Price,
        string? ImageUrl,
        bool IsAvailable,
        Guid CategoryId,
        string CategoryName,
        decimal? AverageRating
    );
}