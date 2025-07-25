﻿namespace MyBlazor.Data
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductPrice * Quantity; // Properti hitung untuk total harga item
    }
}