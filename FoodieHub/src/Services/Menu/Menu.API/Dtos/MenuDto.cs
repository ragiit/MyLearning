using System.Text.Json.Serialization;

namespace Menu.API.Dtos
{
    public record MenuDto
    {
        public Guid Id { get; init; }
        public Guid CategoryId { get; init; }
        public CategoryDto? Category { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string? ImageUrl { get; init; }
        public bool IsAvailable { get; init; }
        public decimal? AverageRating { get; init; }

        [JsonIgnore]
        public IEnumerable<MenuImageDto> Images { get; init; } = [];

        public IEnumerable<string> ImageUrls => Images.Select(x => x.Url);
    }
}