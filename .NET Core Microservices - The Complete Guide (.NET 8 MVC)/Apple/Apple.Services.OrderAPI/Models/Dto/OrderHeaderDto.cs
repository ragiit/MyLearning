﻿namespace Apple.Services.OrderAPI.Models.Dto
{
    public class OrderHeaderDto
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string? CouponCode { get; set; }

        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }

        public IEnumerable<OrderDetailDto>? OrderDetails { get; set; }
    }
}