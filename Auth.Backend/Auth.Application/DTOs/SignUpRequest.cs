using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Application.DTOs
{
    public class SignUpRequest
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("Username")]
        public required string UserName { get; set; }

        [JsonPropertyName("password")]
        public required string Password { get; set; }

        [JsonPropertyName("RememberMe")]
        public bool? RememberMe { get; set; }
    }
}