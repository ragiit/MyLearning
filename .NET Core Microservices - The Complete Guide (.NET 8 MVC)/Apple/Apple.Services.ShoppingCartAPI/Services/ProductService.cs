﻿using Newtonsoft.Json;

namespace Apple.Services.ShoppingCartAPI.Services
{
    public class ProductService(IHttpClientFactory httpClientFactory) : IProductService
    {
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var client = httpClientFactory.CreateClient("ProductAPI");
            var response = await client.GetAsync("/api/products");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(resp.Result.ToString()!);
            }

            return [];
        }
    }
}