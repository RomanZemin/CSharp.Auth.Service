using Auth.Domain.Models;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public partial class JWTService
    {
        public string GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenObject = new RefreshToken
            {
                Token = refreshToken,
                Expiry = DateTime.UtcNow.AddDays(30) // Set a proper expiration time
            };
            _refreshTokens[refreshToken] = refreshTokenObject;
            return refreshToken;
        }
    }
}
