using Apple.Web.Utility;
using static Apple.Web.Utility.SD;

namespace Apple.Web.Service
{
    public class CouponService(IBaseService baseService) : ICouponService
    {
        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode/" + couponCode
            });
        }

        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = couponDto,
                Url = SD.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.PUT,
                Data = couponDto,
                Url = SD.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int id)
        {
            return await baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.DELETE,
                Url = SD.CouponAPIBase + "/api/coupon/" + id
            });
        }
    }
}