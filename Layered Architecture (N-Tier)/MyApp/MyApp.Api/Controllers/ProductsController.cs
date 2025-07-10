namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/products")]
[EnableRateLimiting("fixed")]
public class ProductsController(IProductService service) : ControllerBase
{
    //[HttpGet]
    //[ProducesResponseType(typeof(BaseResponse<IEnumerable<ProductDto>>), 200)]
    //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
    //public async Task<IActionResult> GetAll()
    //{
    //    var products = await service.GetAllAsync();
    //    var dtos = products.Adapt<IEnumerable<ProductDto>>();
    //    return Ok(BaseResponse<IEnumerable<ProductDto>>.Ok(dtos));
    //}

    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<ProductDto>>), 200)]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequest request)
    {
        var (items, total) = await service.GetPagedAsync(request.PageNumber, request.PageSize);
        var dtos = items.Adapt<IEnumerable<ProductDto>>();
        return Ok(PagedResponse<ProductDto>.Create(dtos, request.PageNumber, request.PageSize, total));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await service.GetByIdAsync(id);
        if (product == null)
            return NotFound(BaseResponse<ProductDto>.Fail("Product not found"));

        var dto = product.Adapt<ProductDto>();
        return Ok(BaseResponse<ProductDto>.Ok(dto));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        var product = dto.Adapt<Product>();
        var created = await service.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, BaseResponse<ProductDto>.Ok(created.Adapt<ProductDto>()));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDto dto)
    {
        if (id != dto.Id) return BadRequest(BaseResponse<ProductDto>.Fail("ID mismatch"));

        var product = dto.Adapt<Product>();
        var updated = await service.UpdateAsync(product);
        return Ok(BaseResponse<ProductDto>.Ok(updated.Adapt<ProductDto>()));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return Ok(BaseResponse<string>.Ok("Deleted successfully"));
    }
}