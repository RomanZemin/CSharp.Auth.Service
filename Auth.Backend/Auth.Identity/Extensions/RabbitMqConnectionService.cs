using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

namespace Auth.Identity.Extensions
{
    public class RabbitMqConnectionService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqConnectionService(IConfiguration configuration)
        {
            var rabbitMqUrl = configuration["RabbitMQ:Url"];

            if (string.IsNullOrEmpty(rabbitMqUrl))
            {
                throw new InvalidOperationException("RabbitMQ URL is not configured.");
            }

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitMqUrl)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "MyQueue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public IModel Channel => _channel;

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
