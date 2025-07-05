namespace Apple.Services.ShoppingCartAPI.Models
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }

        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }

        public CartHeader? CartHeader { get; set; }

        [NotMapped]
        public ProductDto? Product { get; set; }
    }
}