namespace Menu.API.Dtos
{
    public record CreateMenuRequest
    {
        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(250, ErrorMessage = "Menu name cannot exceed 250 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [StringLength(2000, ErrorMessage = "Image URL cannot exceed 2000 characters.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public Guid CategoryId { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}