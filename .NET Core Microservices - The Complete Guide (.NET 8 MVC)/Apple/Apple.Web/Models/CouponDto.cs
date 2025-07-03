namespace Apple.Web.Models
{
    public record CouponDto(int Id, string Code, double Discount, int MinAmount);
}