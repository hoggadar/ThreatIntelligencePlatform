namespace ThreatIntelligencePlatform.MessageBroker.Interfaces;

public interface IRabbitMQService
{
    void Publish<T>(string exchange, string routingKey, T message);
    void Subscribe<T>(string queue, Func<T, Task> handler);
}