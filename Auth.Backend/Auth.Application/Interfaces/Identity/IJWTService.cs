using Auth.Domain.Models;
using System.Threading.Tasks;

namespace Auth.Application.Interfaces
{
    public interface IJWTService
    {
        string GenerateJwtToken(IUser user);
        string GenerateRefreshToken();
        string RefreshToken(string refreshToken, IUser user);
        bool ValidateToken(string token);
    }
}
