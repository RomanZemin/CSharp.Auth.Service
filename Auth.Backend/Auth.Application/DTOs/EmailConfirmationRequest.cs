using System.Text.Json.Serialization;

namespace Auth.Application.DTOs
{
    public class EmailConfirmationRequest
    {
        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}