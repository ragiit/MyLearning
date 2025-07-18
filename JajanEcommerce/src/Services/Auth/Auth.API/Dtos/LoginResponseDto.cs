namespace Auth.API.Dtos
{
    public sealed record LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }
}