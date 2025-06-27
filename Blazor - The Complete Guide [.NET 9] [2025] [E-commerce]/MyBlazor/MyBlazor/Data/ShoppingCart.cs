using System.ComponentModel.DataAnnotations.Schema;

namespace MyBlazor.Data
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string ApplicationUserId { get; set; }
        public int Quantity { get; set; } = 1;

        [NotMapped] // Properti ini tidak disimpan ke DB, dihitung saat runtime
        public decimal TotalPrice => (Product?.Price ?? 0) * Quantity;

        public ApplicationUser? ApplicationUser { get; set; }
        public Product? Product { get; set; }
    }
}