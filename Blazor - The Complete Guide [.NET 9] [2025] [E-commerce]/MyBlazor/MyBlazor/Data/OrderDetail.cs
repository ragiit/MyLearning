namespace MyBlazor.Data
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public OrderHeader? OrderHeader { get; set; }
    }
}