namespace Discount.Grpc.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}