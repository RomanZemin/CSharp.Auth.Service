using System.Text.Json.Serialization;

namespace Auth.Application.DTOs
{
    public class ResetPasswordRequest
    {
        [JsonPropertyName("userEmail")]
        public required string UserEmail { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("newPassword")]
        public required string NewPassword { get; set; }
    }
}