namespace Auth.Domain.Models
{
    public interface IUser
    {
        string? Id { get; set; }
        string? Email { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
        DateTime? Birthdate { get; set; }
        string? Refresh_Token { get; set; }
        string? JWT_Token { get; set; }
        string? Expires_At { get; set; }
    }
}