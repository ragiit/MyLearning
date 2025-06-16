using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Judul produk tidak boleh kosong.")]
        [Display(Name = "Judul Produk")]
        public string Title { get; set; }

        // Deskripsi bisa jadi opsional, jadi tidak perlu [Required]
        public string? Description { get; set; }

        [Required(ErrorMessage = "ISBN tidak boleh kosong.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Nama penulis tidak boleh kosong.")]
        [Display(Name = "Penulis")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Harga normal tidak boleh kosong.")]
        [Display(Name = "Harga Normal")]
        [Range(1, 10000000, ErrorMessage = "Harga harus di antara 1 dan 10.000.000.")]
        public double ListPrice { get; set; }

        [Required(ErrorMessage = "Harga untuk 1-50 tidak boleh kosong.")]
        [Display(Name = "Harga (1-50)")]
        [Range(1, 10000000, ErrorMessage = "Harga harus di antara 1 dan 10.000.000.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Harga untuk 50+ tidak boleh kosong.")]
        [Display(Name = "Harga (50+)")]
        [Range(1, 10000000, ErrorMessage = "Harga harus di antara 1 dan 10.000.000.")]
        public double Price50 { get; set; }

        [Required(ErrorMessage = "Harga untuk 100+ tidak boleh kosong.")]
        [Display(Name = "Harga (100+)")]
        [Range(1, 10000000, ErrorMessage = "Harga harus di antara 1 dan 10.000.000.")]
        public double Price100 { get; set; }

        // --- Foreign Key & Navigation Property untuk Category ---

        [Required(ErrorMessage = "Kategori harus dipilih.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; } // Ini adalah Foreign Key

        [ForeignKey("CategoryId")]
        [ValidateNever] // Mencegah validasi pada properti navigasi
        public Category Category { get; set; } = null!; // Ini adalah Navigation Property

        // --- Properti Tambahan untuk Gambar ---
        [Display(Name = "URL Gambar")]
        [ValidateNever]
        public string? ImageUrl { get; set; }
    }
}