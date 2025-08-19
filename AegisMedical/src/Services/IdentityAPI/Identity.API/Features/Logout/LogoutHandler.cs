using BuildingBlocks.CQRS;

namespace Identity.API.Features.Logout;
public sealed record LogoutCommand(string RefreshToken) : ICommand<Unit>;

public sealed class LogoutHandler(
    ApplicationDbContext db) : ICommandHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        //var token = await db.UserRefreshTokens
        //    .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken) ?? throw new UnauthorizedAccessException("Invalid refresh token.");
        //token.IsRevoked = true;
        //await db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}