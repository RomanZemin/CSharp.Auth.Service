﻿using Auth.Domain.Models;
using Auth.Domain.Token;
using Auth.Infrastructure.Identity.Interfaces;
using Auth.Infrastructure.Identity.Models;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public class JWTService : IJWTService
    {

        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        private static readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens = new ConcurrentDictionary<string, RefreshToken>();

        public JWTService(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
            new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
        new Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(Auth.Domain.Token.ClaimTypes.UserId, user.Id ?? string.Empty),
        new Claim(Auth.Domain.Token.ClaimTypes.Jti, Guid.NewGuid().ToString()),
        };

            var payload = new Dictionary<string, object>
        {
            { "iss", _issuer },
            { "aud", _audience },
            { "exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() },
            { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            { "sub", user.Id ?? string.Empty },
            { "email", user.Email ?? string.Empty },
            { Auth.Domain.Token.ClaimTypes.Jti, Guid.NewGuid().ToString() },
            { Auth.Domain.Token.ClaimTypes.UserId, user.Id ?? string.Empty }
        };

            var headerJson = JsonSerializer.Serialize(new { alg = "HS256", typ = "JWT" });
            var payloadJson = JsonSerializer.Serialize(payload);

            var headerEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var payloadEncoded = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            var signature = ComputeHmacSha256($"{headerEncoded}.{payloadEncoded}", Encoding.UTF8.GetBytes(_secretKey));
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

        public bool ValidateToken(string token)
        {
            var parts = token.Split('.');

            if (parts.Length != 3)
                return false;

            var header = parts[0];
            var payload = parts[1];
            var signature = parts[2];

            var key = Encoding.UTF8.GetBytes(_secretKey);
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
