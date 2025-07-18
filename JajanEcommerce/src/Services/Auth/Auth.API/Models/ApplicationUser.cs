using Microsoft.AspNetCore.Identity;

namespace Auth.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}