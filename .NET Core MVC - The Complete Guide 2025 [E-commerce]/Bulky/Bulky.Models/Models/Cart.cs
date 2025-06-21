using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

        [Range(0, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        [NotMapped] // Atribut ini sangat penting!
        public double Price { get; set; }

        [ValidateNever]
        public Product? Product { get; set; }

        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}