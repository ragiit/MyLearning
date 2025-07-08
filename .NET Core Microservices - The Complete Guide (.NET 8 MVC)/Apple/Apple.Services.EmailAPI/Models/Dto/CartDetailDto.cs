namespace Apple.Services.EmailAPI.Models.Dto
{
    public class CartDetailDto
    {
        public int Id { get; set; }

        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }

        public CartHeaderDto? CartHeader { get; set; }
        public ProductDto? Product { get; set; }
    }
}