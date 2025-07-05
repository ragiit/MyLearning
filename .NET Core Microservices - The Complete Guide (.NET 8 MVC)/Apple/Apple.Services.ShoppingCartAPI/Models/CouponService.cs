using Newtonsoft.Json;

namespace Apple.Services.ShoppingCartAPI.Models
{
    public class CouponService(IHttpClientFactory httpClientFactory) : ICouponService
    {
        public async Task<CouponDto?> GetCouponByCodeAsync(string couponCode)
        {
            var client = httpClientFactory.CreateClient("CouponAPI");
            var response = await client.GetAsync($"/api/coupons/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (resp != null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }

            return null;
        }
    }
}