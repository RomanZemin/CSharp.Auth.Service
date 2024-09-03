using Auth.Domain.Models;
using Auth.Domain.Token;
using Auth.Infrastructure.Identity.Interfaces;
using Auth.Infrastructure.Identity.Models;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public partial class JWTService : IJWTService
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
    }
}
