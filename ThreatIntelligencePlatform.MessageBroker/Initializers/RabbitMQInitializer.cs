using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;

namespace ThreatIntelligencePlatform.MessageBroker.Initializers;

public class RabbitMQInitializer
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<RabbitMQInitializer> _logger;

    public RabbitMQInitializer(IRabbitMQService rabbitMQService, ILogger<RabbitMQInitializer> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }
    
    public void Initialize()
    {
        try
        {
            _rabbitMQService.DeclareExchange("main.exchange", "topic");
            
            _rabbitMQService.DeclareQueue("queue1");
            _rabbitMQService.DeclareQueue("queue2");
            
            _rabbitMQService.BindQueue("queue1", "main.exchange", "routing.key1");
            _rabbitMQService.BindQueue("queue2", "main.exchange", "routing.key2");

            _logger.LogInformation("RabbitMQ infrastructure initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ infrastructure");
            throw;
        }
    }
}