namespace Apple.Web.Models
{
    public class OrderDetailDto
    {
        public int Id { get; set; }

        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}