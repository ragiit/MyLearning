namespace Menu.API.Persistence.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(2000)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        // Navigation property (jika diperlukan untuk Eager/Lazy Loading)
        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}