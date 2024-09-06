using Auth.Application.Interfaces.Persistence;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace Auth.Identity.Extensions
{
    class RabbitMqService : IRabbitMqService
    {
        private readonly RabbitMqConnectionService _connectionService;

        public RabbitMqService(RabbitMqConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task SendMessageAsync(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            await SendMessageAsync(message);
        }

        public async Task SendMessageAsync(string message)
        {
            var channel = _connectionService.Channel;

            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() =>
            {
                channel.BasicPublish(exchange: "",
                                     routingKey: "MyQueue",
                                     basicProperties: null,
                                     body: body,
                                     mandatory: false);
            });
        }
    }
}
