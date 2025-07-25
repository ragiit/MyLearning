namespace Menu.API.Dtos
{
    public record MenuImageDto(
        Guid Id,
        Guid MenuId,
        string Url,
        bool IsThumbnail = false  // Untuk menandakan apakah ini gambar thumbnail utama
    );
}