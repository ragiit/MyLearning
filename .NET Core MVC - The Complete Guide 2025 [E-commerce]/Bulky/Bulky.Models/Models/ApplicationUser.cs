using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? CompanyId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        [ValidateNever]
        public Company? Company { get; set; }
    }
}