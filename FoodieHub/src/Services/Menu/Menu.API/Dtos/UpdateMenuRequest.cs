namespace Menu.API.Dtos
{
    public record UpdateMenuRequest
    {
        // ID menu yang akan diupdate
        [Required(ErrorMessage = "Menu ID is required for update.")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Category ID is required.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(250, ErrorMessage = "Menu name cannot exceed 250 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        [Display(Name = "Thumbnail Image")]
        public IFormFile? ImageUrl { get; set; } // Gambar thumbnail

        // Jika Anda ingin beberapa gambar tambahan:
        public List<IFormFile>? AdditionalImages { get; set; }
    }
}