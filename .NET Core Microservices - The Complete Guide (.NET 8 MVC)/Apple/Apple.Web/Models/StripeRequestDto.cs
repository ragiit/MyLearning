namespace Apple.Web.Models
{
    public class StripeRequestDto
    {
        public string StripeSessionUrl { get; set; } = string.Empty;
        public string StripeSessionId { get; set; } = string.Empty;
        public string ApprovedUrl { get; set; } = string.Empty;
        public string CancelledUrl { get; set; } = string.Empty;
        public OrderHeaderDto OrderHeader { get; set; } = new OrderHeaderDto();
    }
}