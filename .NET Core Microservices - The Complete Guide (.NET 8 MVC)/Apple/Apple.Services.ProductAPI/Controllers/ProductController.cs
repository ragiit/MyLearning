using Microsoft.AspNetCore.Authorization;

namespace Apple.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController(AppDbContext db, IWebHostEnvironment webHostEnvironment) : ControllerBase
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
        public async Task<ActionResult<ResponseDto>> Post([FromForm] ProductDto productDto)
        {
            try
            {
                var product = productDto.Adapt<Product>();
                if (productDto.Image != null)
                {
                    // Generate nama file unik dan simpan gambar
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
                    var filePath = Path.Combine(webHostEnvironment.WebRootPath, "ProductImages", fileName);
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productDto.Image.CopyToAsync(fileStream);
                    }
                    // Set URL gambar
                    var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
                    product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
                }
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
        public async Task<ActionResult<ResponseDto>> Put([FromForm] ProductDto productDto)
        {
            try
            {
                var product = productDto.Adapt<Product>();
                if (productDto.Image != null)
                {
                    // Hapus gambar lama jika ada
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldFileName = Path.GetFileName(new Uri(product.ImageUrl).AbsolutePath);
                        var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, "ProductImages", oldFileName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Simpan gambar baru
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDto.Image.FileName);
                    var filePath = Path.Combine(webHostEnvironment.WebRootPath, "ProductImages", fileName);
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productDto.Image.CopyToAsync(fileStream);
                    }
                    var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
                    product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
                }
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
                // Hapus gambar terkait
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldFileName = Path.GetFileName(new Uri(product.ImageUrl).AbsolutePath);
                    var oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, "ProductImages", oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
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