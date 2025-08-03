using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ShoppingWeb.Services
{
    public interface ICurrentUserService
    {
        Task<string?> GetUserIdAsync();

        Task<string?> GetUserNameAsync();

        Task<string?> GetEmailAsync();

        Task<List<string>> GetRolesAsync();

        Task<ClaimsPrincipal> GetUserAsync();
    }

    public class CurrentUserService(AuthenticationStateProvider authProvider) : ICurrentUserService
    {
        private readonly AuthenticationStateProvider _authProvider = authProvider;

        private async Task<ClaimsPrincipal> GetPrincipalAsync()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            return authState.User;
        }

        public async Task<ClaimsPrincipal> GetUserAsync() => await GetPrincipalAsync();

        public async Task<string?> GetUserIdAsync()
        {
            var user = await GetPrincipalAsync();
            return user.FindFirst("sub")?.Value
                ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<string?> GetUserNameAsync()
        {
            var user = await GetPrincipalAsync();
            return user.FindFirst("name")?.Value
                ?? user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public async Task<string?> GetEmailAsync()
        {
            var user = await GetPrincipalAsync();
            return user.FindFirst("email")?.Value
                ?? user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public async Task<List<string>> GetRolesAsync()
        {
            var user = await GetPrincipalAsync();
            return user.FindAll("role").Select(r => r.Value)
                .Concat(user.FindAll(ClaimTypes.Role).Select(r => r.Value))
                .Distinct()
                .ToList();
        }
    }
}