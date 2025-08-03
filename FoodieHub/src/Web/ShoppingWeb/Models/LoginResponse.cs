namespace ShoppingWeb.Models
{
    namespace ShoppingWeb.Models
    {
        public record LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public DateTime RefreshTokenExpiresAt { get; set; }
        }
    }
}