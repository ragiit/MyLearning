using Bulky.Models.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models.ViewModels
{
    public class CartVM
    {
        // Daftar semua item di keranjang
        public IEnumerable<Cart> CartList { get; set; }

        // Properti untuk menyimpan total harga pesanan
        public double OrderTotal { get; set; }

        [NotMapped] // Atribut ini sangat penting!
        public double Price { get; set; }
    }
}