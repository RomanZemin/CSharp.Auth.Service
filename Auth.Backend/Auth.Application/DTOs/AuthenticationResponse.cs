using System.Text.Json.Serialization;

namespace Auth.Application.DTOs
{
    public class AuthenticationResponse
    {
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }

        [JsonPropertyName("errors")]
        public Dictionary<string, string>? Errors { get; set; }

        [JsonPropertyName("data")]
        public UserData? Data { get; set; }

        [JsonPropertyName("access")]
        public AccessToken? Access { get; set; }
    }

    public class UserData
    {
        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("FirstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("LastName")]
        public string? LastName { get; set; }
        [JsonPropertyName("UserName")]
        public string? UserName { get; set; }
    }

    public class AccessToken
    {
        [JsonPropertyName("Refresh_Token")]
        public string? Refresh_Token { get; set; }

        [JsonPropertyName("jwt")]
        public string? Jwt { get; set; }

        [JsonPropertyName("expiresAt")]
        public string? ExpiresAt { get; set; }
    }
}
