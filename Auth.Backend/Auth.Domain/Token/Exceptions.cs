namespace Auth.Domain.Token
{
    public class SecurityTokenException : Exception
    {
        public SecurityTokenException(string message) : base(message) { }
    }
}
