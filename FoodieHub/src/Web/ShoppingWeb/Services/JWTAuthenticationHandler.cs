using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ShoppingWeb.Services
{
    public class JWTAuthenticationHandler : AuthenticationHandler<CustomOption>
    {
        public JWTAuthenticationHandler(IOptionsMonitor<CustomOption> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var t = Request.Cookies["access_token"];
                if (t == null)
                    return AuthenticateResult.NoResult();

                var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(t);
                var identity = new ClaimsIdentity(readJwt.Claims, "JWT");
                var prin = new ClaimsPrincipal(identity);
                var tiket = new AuthenticationTicket(prin, Scheme.Name);

                return AuthenticateResult.Success(tiket);
            }
            catch (Exception)
            {
                return AuthenticateResult.NoResult();
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/login");
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/accessdenied");
            return Task.CompletedTask;
        }
    }

    public class CustomOption : AuthenticationSchemeOptions
    {
    }
}