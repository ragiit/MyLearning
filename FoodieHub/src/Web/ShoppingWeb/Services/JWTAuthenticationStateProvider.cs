using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShoppingWeb.Services
{
    public class JWTAuthenticationStateProvider(AccessTokenService accessTokenService) : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await accessTokenService.GetToken();
                if (string.IsNullOrWhiteSpace(token))
                {
                    return await MarkAsUnauthorize();
                }

                var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var identity = new ClaimsIdentity(readJwt.Claims, "JWT");
                var prin = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(prin));
            }
            catch (Exception)
            {
                return await MarkAsUnauthorize();
            }
        }

        private async Task<AuthenticationState> MarkAsUnauthorize()
        {
            try
            {
                var s = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                NotifyAuthenticationStateChanged(Task.FromResult(s));
                return s;
            }
            catch (Exception)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }
    }
}