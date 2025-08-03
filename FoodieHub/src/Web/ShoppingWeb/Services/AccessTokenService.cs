namespace ShoppingWeb.Services
{
    public class AccessTokenService(CookieService cookieService)
    {
        private readonly string TokenKey = "access_token";

        public async Task SetToken(string token) => await cookieService.SetCookieAsync(TokenKey, token, 1);

        public async Task<string> GetToken() => await cookieService.GetCookieAsync(TokenKey);

        public async Task RemoveToken() => await cookieService.DeleteCookieAsync(TokenKey);
    }
}