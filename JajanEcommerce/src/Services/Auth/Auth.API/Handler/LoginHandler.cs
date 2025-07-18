using Auth.API.Dtos;
using Auth.API.Services.IService;
using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Handler;

public sealed record AuthCommand(LoginRequestDto Login) : ICommand<AuthResult>;
public sealed record AuthResult(LoginResponseDto LoginResponse);

public sealed class AuthCommandValidator : AbstractValidator<AuthCommand>
{
    public AuthCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotNull().WithMessage("Login data must not be null.");

        RuleFor(x => x.Login.Username)
            .NotEmpty().WithMessage("Username or email is required.");

        RuleFor(x => x.Login.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

public sealed class LoginHandler(
    AppDbContext db,
    UserManager<ApplicationUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator)
    : ICommandHandler<AuthCommand, AuthResult>
{
    public async Task<AuthResult> Handle(AuthCommand request, CancellationToken cancellationToken)
    {
        var user = await db.ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.UserName!.ToLower() == request.Login.Username.ToLower(),
                cancellationToken);

        if (user == null)
        {
            return new AuthResult(new LoginResponseDto
            {
                User = null,
                Token = string.Empty
            });
        }

        var isValid = await userManager.CheckPasswordAsync(user, request.Login.Password);

        if (!isValid)
        {
            return new AuthResult(new LoginResponseDto
            {
                User = null,
                Token = string.Empty
            });
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenGenerator.GenerateToken(user, roles);

        var userDto = new UserDto
        {
            ID = user.Id,
            Email = user.Email!,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber
        };

        return new AuthResult(new LoginResponseDto
        {
            User = userDto,
            Token = token
        });
    }
}