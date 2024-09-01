namespace Auth.Domain.Models
{
    public class RefreshToken : IRefreshToken
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}