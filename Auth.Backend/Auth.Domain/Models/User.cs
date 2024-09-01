namespace Auth.Domain.Models
{
    public class User : IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Refresh_Token { get; set; }
        public string? JWT_Token { get; set; }
        public string? Expires_At { get; set; }
    }
}
