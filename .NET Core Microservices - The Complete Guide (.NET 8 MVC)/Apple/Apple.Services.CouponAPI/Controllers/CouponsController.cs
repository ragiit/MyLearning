using Microsoft.AspNetCore.Authorization;

namespace Apple.Services.CouponAPI.Controllers
{
    [Route("api/coupons")]
    [ApiController]
    [Authorize]
    public class CouponController(AppDbContext db) : ControllerBase
    {
        private readonly ResponseDto _response = new();

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> Get()
        {
            try
            {
                var coupons = await db.Coupons.ToListAsync();
                _response.Result = coupons.Adapt<IEnumerable<CouponDto>>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            return Ok(_response);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<ResponseDto>> Get(int id)
        {
            try
            {
                var coupon = await db.Coupons.FindAsync(id);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Coupon with ID {id} not found.";
                    return NotFound(_response);
                }
                _response.Result = coupon.Adapt<CouponDto>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            return Ok(_response);
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public async Task<ActionResult<ResponseDto>> GetByCode(string code)
        {
            try
            {
                var coupon = await db.Coupons.FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Coupon with code '{code}' not found.";
                    return NotFound(_response);
                }
                _response.Result = coupon.Adapt<CouponDto>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            return Ok(_response);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = couponDto.Adapt<Coupon>();
                db.Coupons.Add(coupon);
                await db.SaveChangesAsync();

                _response.Result = coupon.Adapt<CouponDto>();
                return CreatedAtAction(nameof(Get), new { id = coupon.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Put([FromBody] CouponDto couponDto)
        {
            try
            {
                var coupon = couponDto.Adapt<Coupon>();
                db.Coupons.Update(coupon);
                await db.SaveChangesAsync();
                _response.Result = coupon.Adapt<CouponDto>();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            return Ok(_response);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            try
            {
                var coupon = await db.Coupons.FindAsync(id);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Coupon with ID {id} not found.";
                    return NotFound(_response);
                }
                db.Coupons.Remove(coupon);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
            return Ok(_response);
        }
    }
}