using Auth.API.Dtos;
using BuildingBlocks.CQRS;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Auth.API.Handler;

// COMMAND
public sealed record RegisterCommand(RegisterRequestDto Register) : ICommand<RegisterResult>;

// RESULT
public sealed record RegisterResult(RegisterResponseDto RegisterResponse);

// VALIDATOR
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Register.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Register.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters.");

        RuleFor(x => x.Register.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{7,15}$")
            .WithMessage("Invalid phone number format.");

        RuleFor(x => x.Register.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}

// HANDLER
public sealed class RegisterHandler(
    AppDbContext db,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
    : ICommandHandler<RegisterCommand, RegisterResult>
{
    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Register.Email,
            Email = request.Register.Email,
            NormalizedEmail = request.Register.Email.ToUpperInvariant(),
            Name = request.Register.Name,
            PhoneNumber = request.Register.PhoneNumber
        };

        var createResult = await userManager.CreateAsync(user, request.Register.Password);

        if (createResult.Succeeded)
        {
            var role = string.IsNullOrWhiteSpace(request.Register.Role) ? "Customer" : request.Register.Role;

            // Tambahkan role hanya jika ada di DB
            if (await roleManager.RoleExistsAsync(role))
                await userManager.AddToRoleAsync(user, role);

            var userDto = user.Adapt<UserDto>();

            return new RegisterResult(new RegisterResponseDto
            {
                User = userDto,
                Errors = null
            });
        }

        // Jika gagal → kirim daftar error
        var errors = createResult.Errors.Select(e => e.Description).ToList();

        return new RegisterResult(new RegisterResponseDto
        {
            User = null,
            Errors = errors
        });
    }
}
