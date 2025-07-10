using System.ComponentModel.DataAnnotations;

namespace Apple.Web.Models
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(1, 10000)]
        public double Price { get; set; }

        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }

        [AllowedExtensionsAttribute([".jpg", ".png"])]
        public IFormFile? Image { get; set; }

        public int Count { get; set; } = 1;
    }
}