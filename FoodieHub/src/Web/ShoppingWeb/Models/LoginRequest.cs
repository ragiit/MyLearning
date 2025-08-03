namespace ShoppingWeb.Models
{
    public record LoginRequest
    {
        [Required(ErrorMessage = "Email diperlukan.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password diperlukan.")]
        public string Password { get; set; } = string.Empty;
    }
}