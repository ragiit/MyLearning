using Auth.API.Exceptions;
using BuildingBlocks.CQRS;

namespace Auth.API.Features.Login
{
    public sealed record LoginCommand(LoginRequest Login) : ICommand<AuthResult>;
    public sealed record AuthResult(LoginResponse LoginResponse);

    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotNull().WithMessage("Login data must not be null.");

            RuleFor(x => x.Login.Email)
                .NotEmpty().WithMessage("Email is required.");

            RuleFor(x => x.Login.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }

    public sealed class LoginHandler(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
        : ICommandHandler<LoginCommand, AuthResult>
    {
        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await db.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    u => u.Email == request.Login.Email,
                    cancellationToken) ?? throw new LoginNotFoundException(request.Login.Email);

            var isValid = await userManager.CheckPasswordAsync(user, request.Login.Password);

            if (!isValid)
            {
                throw new LoginNotFoundException(request.Login.Email);
            }

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtTokenGenerator.GenerateToken(user, roles);

            var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

            var expiresAt = DateTime.UtcNow.AddMinutes(15);
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            //var oldTokens = await db.UserRefreshTokens
            //    .Where(x => x.UserId == user.Id && !x.IsRevoked && x.ExpiresAt > DateTime.UtcNow)
            //    .ToListAsync(cancellationToken: cancellationToken);

            //foreach (var old in oldTokens)
            //{
            //    old.IsRevoked = true;
            //}

            //db.UserRefreshTokens.Add(new UserRefreshToken
            //{
            //    UserId = user.Id,
            //    Token = refreshToken,
            //    ExpiresAt = refreshTokenExpiresAt
            //});

            //await db.SaveChangesAsync(cancellationToken);

            return new AuthResult(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            });
        }
    }
}