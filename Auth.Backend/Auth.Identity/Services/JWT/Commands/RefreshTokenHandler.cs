using Auth.Domain.Models;
using Auth.Infrastructure.Identity.Models;
using Auth.Domain.Token;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public partial class JWTService
    {
        public string RefreshToken(string refreshToken, ApplicationUser user)
        {
            if (_refreshTokens.TryGetValue(refreshToken, out var tokenObject) && tokenObject.Expiry > DateTime.UtcNow)
            {
                _refreshTokens.TryRemove(refreshToken, out _);
                var newJwtToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();
                _refreshTokens[newRefreshToken] = new RefreshToken
                {
                    Token = newRefreshToken,
                    Expiry = DateTime.UtcNow.AddDays(30) // Set a proper expiration time
                };

                return newJwtToken;
            }
            else
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
        }
    }
}
