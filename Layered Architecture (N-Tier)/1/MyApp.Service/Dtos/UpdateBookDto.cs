using System.ComponentModel.DataAnnotations;

namespace MyApp.Service.DTOs
{
    // DTO untuk memperbarui buku
    public class UpdateBookDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        [Range(1000, 9999)]
        public int PublicationYear { get; set; }
    }
}