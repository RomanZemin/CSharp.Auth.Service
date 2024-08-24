using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Auth.Application.DTOs
{
    public class SignUpRequest
    {
        [JsonPropertyName("email")]
        [Required]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
    }
}