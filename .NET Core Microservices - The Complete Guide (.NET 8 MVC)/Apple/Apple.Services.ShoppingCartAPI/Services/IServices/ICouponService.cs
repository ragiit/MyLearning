namespace Apple.Services.ShoppingCartAPI.Services.IServices
{
    public interface ICouponService
    {
        Task<CouponDto?> GetCouponByCodeAsync(string couponCode);
    }
}