using BuildingBlocks.CQRS;

namespace Identity.API.Features.Register
{
    public sealed record RegisterCommand(RegisterRequest RegisterRequest) : ICommand<RegisterResponse>;

    public sealed class RegisterHandler(
        UserManager<ApplicationUser> userManager)
        : ICommandHandler<RegisterCommand, RegisterResponse>
    {
        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (request.RegisterRequest.Password != request.RegisterRequest.ConfirmPassword)
                throw new ArgumentException("Password and Confirm Password do not match.");

            var existingUser = await userManager.FindByEmailAsync(request.RegisterRequest.Email);
            if (existingUser is not null)
                throw new InvalidOperationException("Email already exists.");

            var user = new ApplicationUser
            {
                UserName = request.RegisterRequest.Email,
                Email = request.RegisterRequest.Email,
                Name = request.RegisterRequest.Name,
                PhoneNumber = request.RegisterRequest.PhoneNumber
            };

            var result = await userManager.CreateAsync(user, request.RegisterRequest.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            // Default assign role Customer
            await userManager.AddToRoleAsync(user, IdentityRoles.Pasien);

            return new RegisterResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };
        }
    }
}