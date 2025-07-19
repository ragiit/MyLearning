namespace Menu.API.Dtos
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public List<string> Categories { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Carbo { get; set; }
        public decimal Protein { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreateMenuItemDto
    {
        public List<string> Categories { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Carbo { get; set; }
        public decimal Protein { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateMenuItemDto : CreateMenuItemDto
    {
        public Guid Id { get; set; }
    }
}