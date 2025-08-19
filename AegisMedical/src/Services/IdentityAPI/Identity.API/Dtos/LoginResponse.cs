namespace Identity.API.Dtos
{
    public record LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        [JsonIgnore]
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}