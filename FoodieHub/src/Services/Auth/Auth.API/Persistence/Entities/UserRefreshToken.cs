namespace Auth.API.Persistence.Entities;

public class UserRefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = default!;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    public string DeviceId { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = default!;
}