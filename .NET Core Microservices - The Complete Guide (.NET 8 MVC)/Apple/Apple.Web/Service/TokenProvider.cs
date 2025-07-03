namespace Apple.Web.Service
{
    public class TokenProvider(IHttpContextAccessor contextAccessor) : ITokenProvider
    {
        public void ClearToken()
        {
            // Menghapus cookie token dari browser.
            contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;
            // Mencoba membaca nilai token dari cookie yang masuk.
            contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token);
            return token;
        }

        public void SetToken(string token)
        {
            // Menambahkan atau memperbarui cookie token di browser.
            // Cookie akan berlaku selama sesi browser.
            contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
        }
    }
}