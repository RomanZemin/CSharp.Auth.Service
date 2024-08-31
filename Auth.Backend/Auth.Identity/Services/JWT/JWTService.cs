using Auth.Infrastructure.Identity.Models;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public static class JWTService
    {
        private const string SecretKey = "881d43375578e3020726f4d36a5e779d40d3f972e1f3000090567237bca693bb61e4a339df26d38e98561ce5ed8b82f50d4e299a08ee07638c3a197c6c97f7dc";
        private const string Issuer = "WebMonsters";
        private const string Audience = "http://localhost:5250/api";
        private static readonly ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();

        public static string GenerateJwtToken(ApplicationUser user)
        {
            var header = new Dictionary<string, object>
            {
                { "alg", "HS256" },
                { "typ", "JWT" }
            };

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("userId", user.Id)
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", Issuer },
                { "aud", Audience },
                { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() },
                { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "sub", user.Id },
                { "email", user.Email },
                { "jti", Guid.NewGuid().ToString() },
                { "userId", user.Id }
            };

            foreach (var claim in claims)
            {
                payload[claim.Type] = claim.Value;
            }

            var headerJson = JsonSerializer.Serialize(header);
            var payloadJson = JsonSerializer.Serialize(payload);

            var headerEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var payloadEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            var signature = ComputeHmacSha256($"{headerEncoded}.{payloadEncoded}", Encoding.UTF8.GetBytes(SecretKey));

            var signatureEncoded = Base64UrlEncode(signature);

            return $"{headerEncoded}.{payloadEncoded}.{signatureEncoded}";
        }

        public static string GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();
            _refreshTokens[refreshToken] = refreshToken;
            return refreshToken;
        }

        public static string RefreshToken(string refreshToken, ApplicationUser user)
        {
            if (_refreshTokens.TryGetValue(refreshToken, out string? storedToken) && storedToken == refreshToken)
            {
                _refreshTokens.TryRemove(refreshToken, out _);
                var newJwtToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();
                _refreshTokens[newRefreshToken] = newRefreshToken;

                Console.WriteLine($"Generated new Refresh Token: {newRefreshToken}");

                return newJwtToken;
            }
            else
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
        }

        public static bool ValidateToken(string token)
        {
            var parts = token.Split('.');

            if (parts.Length != 3)
                return false;

            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];

            var key = Encoding.UTF8.GetBytes(SecretKey);
            var computedSignature = ComputeHmacSha256($"{header}.{payload}", key);
            var computedSignatureEncoded = Base64UrlEncode(computedSignature);

            if (computedSignatureEncoded != signature)
                return false;

            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var payloadData = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);

            if (payloadData == null || !payloadData.TryGetValue("exp", out var exp) || !long.TryParse(exp.ToString(), out var expSeconds))
                return false;

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds);

            return expirationTime > DateTimeOffset.UtcNow;
        }

        private static byte[] ComputeHmacSha256(string data, byte[] key)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
            }
            return Convert.FromBase64String(output);
        }
    }

    public class SecurityTokenException : Exception
    {
        public SecurityTokenException(string message) : base(message) { }
    }
}
