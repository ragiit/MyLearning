using BuildingBlocks.CQRS;

namespace Identity.API.Features.Logout;

public sealed record LogoutAllCommand(string UserId) : ICommand<Unit>;

public sealed class LogoutAllHandler(
    ApplicationDbContext db) : ICommandHandler<LogoutAllCommand, Unit>
{
    public async Task<Unit> Handle(LogoutAllCommand request, CancellationToken cancellationToken)
    {
        //var tokens = await db.UserRefreshTokens
        //    .Where(x => x.UserId == request.UserId && !x.IsRevoked)
        //    .ToListAsync(cancellationToken);

        //foreach (var token in tokens)
        //{
        //    token.IsRevoked = true;
        //}

        //await db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}