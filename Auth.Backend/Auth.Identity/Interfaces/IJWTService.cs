using Auth.Domain.Models;
using Auth.Infrastructure.Identity.Models;

namespace Auth.Infrastructure.Identity.Interfaces
{
    public interface IJWTService
    {
        string GenerateJwtToken(ApplicationUser user);
        string GenerateRefreshToken();
        string RefreshToken(string refreshToken, ApplicationUser user);
        bool ValidateToken(string token);
    }
}
