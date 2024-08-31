using Auth.Infrastructure.Identity.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Concurrent;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public class JWTService
    {
        private const string SecretKey = "881d43375578e3020726f4d36a5e779d40d3f972e1f3000090567237bca693bb61e4a339df26d38e98561ce5ed8b82f50d4e299a08ee07638c3a197c6c97f7dc";
        private const string Issuer = "WebMonsters"; //Издателель
        private const string Audience = "http://localhost:5250/api";
        private static readonly ConcurrentDictionary<string, string> _refreshTokens = new ConcurrentDictionary<string, string>();

        public string GenerateJwtToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Unix timestamp
                new Claim("userId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();
            // В реальном приложении нужно сохранять refresh-токены в безопасное хранилище, например, в базе данных.
            _refreshTokens[refreshToken] = refreshToken;
            return refreshToken;
        }

        public string RefreshToken(string refreshToken, ApplicationUser user)
        {
            // Проверяем, существует ли refreshToken в хранилище
            if (_refreshTokens.TryGetValue(refreshToken, out string? storedToken) && storedToken == refreshToken)
            {
                // Удаляем использованный refreshToken из хранилища
                _refreshTokens.TryRemove(refreshToken, out _);

                // Генерируем новый JWT токен
                var newJwtToken = GenerateJwtToken(user);

                // Генерируем новый refreshToken и сохраняем его в хранилище
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

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    ClockSkew = TimeSpan.Zero // Убирает задержку времени между сервером и клиентом
                }, out SecurityToken validatedToken);

                // Проверка срока действия токена
                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token has expired");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }
    }
}
