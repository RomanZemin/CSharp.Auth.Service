using System.Text.Json.Serialization;

namespace Auth.Application.DTOs
{
    public class TokenResponse : AuthenticationResponse
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}