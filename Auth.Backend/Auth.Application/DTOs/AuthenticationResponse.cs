﻿using System.Text.Json.Serialization;

namespace Auth.Application.DTOs
{
    public class AuthenticationResponse
    {
        [JsonPropertyName("succeeded")]
        public bool Succeeded { get; set; }
        
        [JsonPropertyName("errors")]
        public Dictionary<string, string> Errors { get; set; }
    }
}
