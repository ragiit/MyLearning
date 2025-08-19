namespace Identity.API.Dtos;

public record RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}