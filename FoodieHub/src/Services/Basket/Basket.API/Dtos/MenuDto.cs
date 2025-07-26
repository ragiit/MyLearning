namespace Basket.API.Dtos
{
    public record MenuDto(
        Guid Id,
        string Name,
        decimal Price,
        string? ImageUrl // Hanya properti yang dibutuhkan dari Menu API
                         // Properti lain seperti Description, IsAvailable, CategoryId, CategoryName, AverageRating tidak perlu jika tidak dipakai
    );
}