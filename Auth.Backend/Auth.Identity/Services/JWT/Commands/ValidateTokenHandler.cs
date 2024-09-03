using System.Text;
using System.Text.Json;

namespace Auth.Infrastructure.Identity.Services.JWT
{
    public partial class JWTService
    {
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
    }
}
