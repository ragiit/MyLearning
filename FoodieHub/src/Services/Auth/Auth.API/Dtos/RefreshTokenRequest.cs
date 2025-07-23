namespace Auth.API.Dtos;

public record RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}