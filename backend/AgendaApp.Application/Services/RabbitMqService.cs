using AgendaApp.Application.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace AgendaApp.Application.Services
{
    public class RabbitMqService : IRabbitMqService, IAsyncDisposable
    {
        private readonly string _hostname = "rabbitmq";
        private readonly string _queueName = "user-created-queue";
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }
        public async Task SendMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await Task.Run(() =>
                _channel.BasicPublish(exchange: "",
                                      routingKey: _queueName,
                                      basicProperties: null,
                                      body: body)
            );
        }

        public async ValueTask DisposeAsync()
        {
            await Task.WhenAll(
                Task.Run(() => _channel?.Dispose()),
                Task.Run(() => _connection?.Dispose())
            );
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }
    }
}