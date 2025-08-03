using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace ShoppingWeb.Services
{
    public class CookieService(IJSRuntime jsRuntime)
    {
        public async Task SetCookieAsync(string key, string value, int day)
        {
            await jsRuntime.InvokeVoidAsync("setCookie", key, value, day);
        }

        public async Task<string?> GetCookieAsync(string key)
        {
            return await jsRuntime.InvokeAsync<string>("getCookie", key);
        }

        public async Task DeleteCookieAsync(string key)
        {
            await jsRuntime.InvokeVoidAsync("deleteCookie", key);
        }
    }
}