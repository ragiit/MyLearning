namespace Apple.Web.Controllers
{
    public class ProductController(IProductService productService) : Controller
    {
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto>? list = [];
            ResponseDto? response = await productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        public async Task<IActionResult> ProductUpsert(int? productId)
        {
            ProductDto model = new();
            if (productId.HasValue && productId.Value > 0)
            {
                ResponseDto? response = await productService.GetProductByIdAsync(productId.Value);
                if (response != null && response.IsSuccess)
                {
                    model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                }
                else
                {
                    TempData["error"] = response?.Message;
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductUpsert(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response;
                if (model.Id == 0)
                {
                    response = await productService.CreateProductAsync(model);
                    if (response.IsSuccess)
                        TempData["success"] = "Product created successfully";
                }
                else
                {
                    response = await productService.UpdateProductAsync(model);
                    if (response.IsSuccess)
                        TempData["success"] = "Product updated successfully";
                }

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int productId)
        {
            ResponseDto? response = await productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            ResponseDto? response = await productService.DeleteProductAsync(productDto.Id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDto);
        }
    }
}