using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ThreatIntelligencePlatform.Configuration.RabbitMQSettings;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;

namespace ThreatIntelligencePlatform.MessageBroker.Services;

public class RabbitMQService : IRabbitMQService
{
    private readonly RabbitMQSettings _rabbitMQSettings;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    
    public RabbitMQService(IOptions<RabbitMQSettings> rabbitMQSettings)
    {
        _rabbitMQSettings = rabbitMQSettings.Value;
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMQSettings.HostName,
            Port = _rabbitMQSettings.Port,
            UserName = _rabbitMQSettings.UserName,
            Password = _rabbitMQSettings.Password,
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(string exchange, string routingKey, T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(exchange, routingKey, null, body);
    }

    public void Subscribe<T>(string queue, Func<T, Task> handler)
    {
        _channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
            await handler(message);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue, false, consumer);
    }
}