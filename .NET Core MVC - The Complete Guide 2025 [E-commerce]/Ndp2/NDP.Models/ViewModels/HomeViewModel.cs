using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NDP.Models.ViewModels
{
    // Models/HomeViewModel.cs
    public class HomeViewModel
    {
        public List<Product> LatestProducts { get; set; }
        public List<Portfolio> Portfolios { get; set; }
    }

    // Models/Product.cs
    public class Product
    {
        private string _name = string.Empty;
        private string _slug = string.Empty;

        [Key]
        public Guid Id { get; set; }

        public Guid CategoryId { get; set; }

        [Required]
        [Column(TypeName = "varchar(500)")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                if (string.IsNullOrWhiteSpace(_slug) || _slug == GenerateSlug(string.Empty))
                {
                    _slug = GenerateSlug(_name);
                }
            }
        }

        public string Slug
        {
            get => _slug;
            set => _slug = string.IsNullOrWhiteSpace(value) ? GenerateSlug(Name) : value;
        }

        [Column(TypeName = "varchar(100)")]
        public string? ShortDescription { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string? FullDescription { get; set; } = string.Empty;

        public decimal Price { get; set; } = 0.0m;

        [Column(TypeName = "varchar(1000)")]
        public string ImageUrl { get; set; } = "https://placehold.co/400x300/E2E8F0/A0AEC0?text=Produk";

        [Column(TypeName = "varchar(100)")]
        public string? Tag { get; set; } = string.Empty;

        [Column(TypeName = "varchar(50)")]
        public string WhatsAppNumber { get; set; } = string.Empty;

        public List<string> ImageUrls { get; set; } = new List<string>();
        public string? PrimaryImageUrl => ImageUrls.FirstOrDefault() ?? "https://placehold.co/400x300/E2E8F0/A0AEC0?text=Produk";

        public int Stock { get; set; } = 10;
        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        private static string GenerateSlug(string name, string? idSuffix = null, int maxLength = 100)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return !string.IsNullOrWhiteSpace(idSuffix) ? idSuffix.Substring(0, Math.Min(idSuffix.Length, 8)) : Guid.NewGuid().ToString("N").Substring(0, 8);
            }

            string str = name.ToLowerInvariant();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();
            str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            str = Regex.Replace(str, @"\s", "-");

            if (string.IsNullOrEmpty(str))
            {
                return !string.IsNullOrWhiteSpace(idSuffix) ? idSuffix.Substring(0, Math.Min(idSuffix.Length, 8)) : Guid.NewGuid().ToString("N").Substring(0, 8);
            }
            return str;
        }

        private string SanitizeAndSetSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return GenerateSlug(this.Name); // Jika value kosong, generate dari nama

            string str = value.ToLowerInvariant();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();
            str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return string.IsNullOrEmpty(str) ? GenerateSlug(this.Name) : str; // Fallback jika hasil sanitasi kosong
        }

        // Metode untuk memanggil slug generation secara eksplisit jika diperlukan
        public void EnsureSlug(bool forceRegenerate = false, bool useIdAsSuffixIfNameEmpty = true)
        {
            string idSuffixForSlug = useIdAsSuffixIfNameEmpty ? this.Id.ToString() : null;
            if (forceRegenerate || string.IsNullOrWhiteSpace(_slug) || _slug == GenerateSlug(string.Empty, idSuffixForSlug))
            {
                _slug = GenerateSlug(this.Name, idSuffixForSlug);
            }
        }

        public Category? Category { get; set; }
    }

    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100, ErrorMessage = "Nama kategori maksimal 100 karakter.")]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //public ICollection<Product> Products { get; set; } = [];
    }

    // Models/Portfolio.cs
    public class Portfolio
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "varchar(1000)")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "varchar(500)")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}