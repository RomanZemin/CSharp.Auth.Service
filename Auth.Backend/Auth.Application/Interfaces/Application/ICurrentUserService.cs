namespace Auth.Application.Interfaces.Application
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}
