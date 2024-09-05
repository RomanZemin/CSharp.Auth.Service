using Auth.Infrastructure.Identity.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public partial class JWTService
    {
        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                //new Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? string.Empty),
                //new Claim(Auth.Domain.Token.ClaimTypes.UserId, user.Id ?? string.Empty),
                new Claim(Auth.Domain.Token.ClaimTypes.Jti, Guid.NewGuid().ToString()),
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", _issuer },
                { "aud", _audience },
                { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() },
                { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "sub", user.Id ?? string.Empty },
                //{ "email", user.Email ?? string.Empty },
                { Auth.Domain.Token.ClaimTypes.Jti, Guid.NewGuid().ToString() },
                //{ Auth.Domain.Token.ClaimTypes.UserId, user.Id ?? string.Empty }
            };

            var headerJson = JsonSerializer.Serialize(new { alg = "HS256", typ = "JWT" });
            var payloadJson = JsonSerializer.Serialize(payload);

            var headerEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var payloadEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            var signature = ComputeHmacSha256($"{headerEncoded}.{payloadEncoded}", Encoding.UTF8.GetBytes(_secretKey));
            var signatureEncoded = Base64UrlEncode(signature);

            return $"{headerEncoded}.{payloadEncoded}.{signatureEncoded}";
        }
    }
}
