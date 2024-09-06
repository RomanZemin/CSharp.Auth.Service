namespace Auth.Application.Extensions.RabbitMQ
{
    public class RabbitMqMessage
    {
        public required string UserName { get; set; }
        public required string UserId { get; set; }
    }
}