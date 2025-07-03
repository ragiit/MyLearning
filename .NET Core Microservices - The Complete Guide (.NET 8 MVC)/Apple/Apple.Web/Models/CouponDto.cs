namespace Apple.Web.Models
{
    public record CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public double Discount { get; set; }
        public int MinAmount { get; set; }
    }
}