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
            _rabbitMQService.DeclareExchange("ioc.raw", "topic");
            _rabbitMQService.DeclareExchange("ioc.normalized", "topic");
            _rabbitMQService.DeclareExchange("ioc.relevant", "topic");
            
            _rabbitMQService.DeclareQueue("ioc.raw.queue");
            _rabbitMQService.DeclareQueue("ioc.normalized.queue");
            _rabbitMQService.DeclareQueue("ioc.relevant.queue");
            
            _rabbitMQService.BindQueue("ioc.raw.queue", "ioc.raw", "ioc.raw.*");
            _rabbitMQService.BindQueue("ioc.normalized.queue", "ioc.normalized", "ioc.normalized.*");
            _rabbitMQService.BindQueue("ioc.relevant.queue", "ioc.relevant", "ioc.relevant.*");

            _logger.LogInformation("RabbitMQ infrastructure initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ infrastructure");
            throw;
        }
    }
}