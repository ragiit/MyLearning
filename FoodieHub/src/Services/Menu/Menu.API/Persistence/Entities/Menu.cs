using System.ComponentModel.DataAnnotations.Schema;

namespace Menu.API.Persistence.Entities
{
    public class Menu
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [StringLength(2000)]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Column(TypeName = "decimal(3, 2)")]
        public decimal? AverageRating { get; set; } // Nullable

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Category? Category { get; set; }
        public ICollection<MenuImage> Images { get; set; } = [];
    }
}