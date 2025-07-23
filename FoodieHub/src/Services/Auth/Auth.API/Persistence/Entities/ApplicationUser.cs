namespace Auth.API.Persistence.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}