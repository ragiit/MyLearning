namespace MyBlazor.Data
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 10000.00, ErrorMessage = "Price must be between 0.01 and 10000.00")]
        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public Category? Category { get; set; }
    }
}