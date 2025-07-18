namespace Auth.API.Dtos
{
    public sealed record RegisterResponseDto
    {
        public UserDto User { get; set; } = new();
        public List<string> Errors { get; set; } = [];
    }
}