namespace Auth.Application.Interfaces.Persistence
{
    public interface IRabbitMqService
    {
        Task SendMessageAsync(object obj);
        Task SendMessageAsync(string message);
    }
}
