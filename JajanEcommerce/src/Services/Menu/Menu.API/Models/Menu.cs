namespace Menu.API.Models
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public List<string> Categories { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Carbo { get; set; }
        public decimal Protein { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}