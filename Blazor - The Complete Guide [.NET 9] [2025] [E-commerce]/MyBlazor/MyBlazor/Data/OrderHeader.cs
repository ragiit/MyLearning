namespace MyBlazor.Data
{
    public class OrderHeader
    {
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        [Display(Name = "Order Total")]
        public double OrderTotal { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Order Status")]
        public string Status { get; set; } = "Pending";

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; } = [];
    }
}