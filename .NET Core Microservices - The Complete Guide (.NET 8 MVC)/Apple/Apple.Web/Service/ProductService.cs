namespace Apple.Web.Service
{
    public class ProductService(IBaseService baseService) : IProductService
    {
        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.ProductAPIBase + "/api/products"
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.ProductAPIBase + "/api/products/" + id
            });
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/products"
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.PUT,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/products"
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.DELETE,
                Url = SD.ProductAPIBase + "/api/products/" + id
            });
        }
    }
}