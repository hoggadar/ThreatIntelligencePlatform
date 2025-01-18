namespace ThreatIntelligencePlatform.MessageBroker.Interfaces;

public interface IRabbitMQService
{
    void DeclareExchange(string exchangeName, string exchangeType, bool durable = true);
    void DeclareQueue(string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false);
    void BindQueue(string queueName, string exchangeName, string routingKey);
    void Publish<T>(string exchangeName, string routingKey, T message) where T : class;
    void Subscribe<T>(string queueName, Func<T, Task> messageHandler, ushort prefetchCount = 1) where T : class;
}    