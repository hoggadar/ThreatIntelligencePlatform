using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.Utils;

namespace ThreatIntelligencePlatform.Service.Database;

public class IoCWriterWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<IoCWriterWorker> _logger;

    public IoCWriterWorker(IRabbitMQService rabbitMQService, ILogger<IoCWriterWorker> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<IoCDto>("ioc.relevant.queue", async (ioc) =>
        {
            var formattedIoC = IoCFormatter.Format(ioc);
            try
            {
                _logger.LogInformation("Recorded IoC: {@FormattedIoC}", formattedIoC);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording IoC: {@FormattedIoC}", formattedIoC);
            }
        });
        
        return Task.CompletedTask;
    }
}