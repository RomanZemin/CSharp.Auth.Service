namespace Auth.Domain.Models
{
    public interface IRefreshToken
    {
        string? Token { get; }
        DateTime Expiry { get; }
    }
}
