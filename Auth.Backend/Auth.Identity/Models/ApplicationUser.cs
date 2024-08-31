using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Refresh_Token { get; set; }
        public string? JWT_Token { get; set; }
        public string? Expires_At { get; set; }
    }
}