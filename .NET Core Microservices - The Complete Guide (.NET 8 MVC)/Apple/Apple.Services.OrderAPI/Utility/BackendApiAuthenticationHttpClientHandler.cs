using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Apple.Services.OrderAPI.Utility
{
    public class BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor contextAccessor) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Mendapatkan token dari konteks HTTP yang masuk (dari client/web).
            var token = await contextAccessor.HttpContext.GetTokenAsync("access_token");

            // 2. Jika token ada, tambahkan ke header "Authorization" pada request yang keluar.
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 3. Lanjutkan mengirim request ke service tujuan (misal: ProductAPI).
            return await base.SendAsync(request, cancellationToken);
        }
    }
}