namespace Apple.Services.CouponAPI.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public double Discount { get; set; }
        public int MinAmount { get; set; }
    }
}