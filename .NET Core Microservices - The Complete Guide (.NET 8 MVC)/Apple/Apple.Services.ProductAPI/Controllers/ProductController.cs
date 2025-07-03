using Microsoft.AspNetCore.Authorization;

namespace Apple.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController(AppDbContext db) : ControllerBase
    {
        private readonly ResponseDto _response = new();

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> Get()
        {
            try
            {
                var products = await db.Products.ToListAsync();
                _response.Result = products.Adapt<IEnumerable<ProductDto>>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
            return Ok(_response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> Get(int id)
        {
            try
            {
                var product = await db.Products.FindAsync(id);
                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product not found.";
                    return NotFound(_response);
                }
                _response.Result = product.Adapt<ProductDto>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] ProductDto productDto)
        {
            try
            {
                var product = productDto.Adapt<Product>();
                await db.Products.AddAsync(product);
                await db.SaveChangesAsync();
                _response.Result = product.Adapt<ProductDto>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Put([FromBody] ProductDto productDto)
        {
            try
            {
                var product = productDto.Adapt<Product>();
                db.Products.Update(product);
                await db.SaveChangesAsync();
                _response.Result = product.Adapt<ProductDto>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            try
            {
                var product = await db.Products.FindAsync(id);
                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Product not found.";
                    return NotFound(_response);
                }
                db.Products.Remove(product);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}