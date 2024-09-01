using Auth.Application.Interfaces;
using Auth.Application.Interfaces.Identity;
using Auth.Domain.Models;
using Auth.Domain.Token;
using Auth.Infrastructure.Identity.Services.JWTService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Auth.Identity.Services.JWT
{
    public class JWTService : IJWTService
    {
        private readonly JWTSettings _jwtSettings;
        private static readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens = new ConcurrentDictionary<string, RefreshToken>();

        public JWTService(JWTSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateJwtToken(IUser user)
        {
            // Обратите внимание на правильное использование ClaimTypes
            var claims = new[]
            {
                new Claim(Domain.Token.ClaimTypes.NameIdentifier, user.Id), // Обновлено
                new Claim(Domain.Token.ClaimTypes.Email, user.Email), // Обновлено
                new Claim(Domain.Token.ClaimTypes.UserId, user.Id), // Обновлено
                new Claim(Domain.Token.ClaimTypes.Name, Guid.NewGuid().ToString()) // Обновлено
            };

            var payload = new Dictionary<string, object>
            {
                { "iss", _jwtSettings.Issuer },
                { "aud", _jwtSettings.Audience },
                { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() },
                { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "sub", user.Id },
                { "email", user.Email },
                { Domain.Token.ClaimTypes.Name, Guid.NewGuid().ToString() },
                { Domain.Token.ClaimTypes.UserId, user.Id }
            };

            var headerJson = JsonSerializer.Serialize(new { alg = "HS256", typ = "JWT" });
            var payloadJson = JsonSerializer.Serialize(payload);

            var headerEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var payloadEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            var signature = ComputeHmacSha256($"{headerEncoded}.{payloadEncoded}", Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signatureEncoded = Base64UrlEncode(signature);

            return $"{headerEncoded}.{payloadEncoded}.{signatureEncoded}";
        }

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

        public string RefreshToken(string refreshToken, IUser user)
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

        public bool ValidateToken(string token)
        {
            var parts = token.Split('.');

            if (parts.Length != 3)
                return false;

            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];

            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
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

        private byte[] ComputeHmacSha256(string data, byte[] key)
        {
            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private byte[] Base64UrlDecode(string input)
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
}
