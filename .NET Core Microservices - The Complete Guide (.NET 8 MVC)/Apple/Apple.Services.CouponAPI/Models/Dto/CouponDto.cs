namespace Apple.Services.CouponAPI.Models.Dto
{
    public record CouponDto(int Id, string Code, double Discount, int MinAmount);
}