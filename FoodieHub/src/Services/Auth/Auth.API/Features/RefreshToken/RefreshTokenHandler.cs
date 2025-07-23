using BuildingBlocks.CQRS;

namespace Auth.API.Features.RefreshToken;
public sealed record RefreshTokenCommand(RefreshTokenRequest Request) : ICommand<LoginResponse>;

public sealed class RefreshTokenHandler(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator)
    : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        //var storedToken = await db.UserRefreshTokens
        //    .Include(x => x.User)
        //    .FirstOrDefaultAsync(x =>
        //        x.Token == request.Request.RefreshToken &&
        //        !x.IsRevoked &&
        //        x.ExpiresAt >= DateTime.UtcNow,
        //        cancellationToken) ?? throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        //var user = storedToken.User;

        //var roles = await userManager.GetRolesAsync(user);
        //var accessToken = jwtTokenGenerator.GenerateToken(user, roles);
        //var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

        //// Revoke old refresh token
        //storedToken.IsRevoked = true;

        //// Add new one
        //db.UserRefreshTokens.Add(new UserRefreshToken
        //{
        //    UserId = user.Id,
        //    Token = newRefreshToken,
        //    ExpiresAt = DateTime.UtcNow.AddDays(7)
        //});

        //await db.SaveChangesAsync(cancellationToken);

        //return new LoginResponse
        //{
        //    Token = accessToken,
        //    RefreshToken = newRefreshToken,
        //    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        //    RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        //};

        return null;
    }
}