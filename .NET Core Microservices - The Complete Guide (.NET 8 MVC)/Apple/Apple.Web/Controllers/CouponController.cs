namespace Apple.Web.Controllers
{
    public class CouponController(ICouponService couponService) : Controller
    {
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? list = [];
            ResponseDto? response = await couponService.GetAllCouponsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        // Action GET untuk menangani Create dan Update
        public async Task<IActionResult> CouponUpsert(int? couponId)
        {
            CouponDto model = new();
            // Jika couponId ada, berarti ini adalah mode Update
            if (couponId.HasValue && couponId.Value > 0)
            {
                ResponseDto? response = await couponService.GetCouponByIdAsync(couponId.Value);
                if (response != null && response.IsSuccess)
                {
                    model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                }
                else
                {
                    TempData["error"] = response?.Message;
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            // Jika tidak ada couponId, ini adalah mode Create, kirim model kosong
            return View(model);
        }

        // Action POST untuk menangani Create dan Update
        [HttpPost]
        public async Task<IActionResult> CouponUpsert(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response;
                // Jika Id = 0, panggil Create
                if (model.Id == 0)
                {
                    response = await couponService.CreateCouponAsync(model);
                    if (response.IsSuccess)
                        TempData["success"] = "Coupon created successfully";
                }
                // Jika Id ada, panggil Update
                else
                {
                    response = await couponService.UpdateCouponAsync(model);
                    if (response.IsSuccess)
                        TempData["success"] = "Coupon updated successfully";
                }

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDto? response = await couponService.GetCouponByIdAsync(couponId);

            if (response != null && response.IsSuccess)
            {
                CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            ResponseDto? response = await couponService.DeleteCouponAsync(couponDto.Id);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(couponDto);
        }
    }
}