﻿namespace Apple.Web.Models
{
    public class CartDto
    {
        public CartHeaderDto? CartHeader { get; set; }
        public List<CartDetailDto>? CartDetails { get; set; }
    }
}