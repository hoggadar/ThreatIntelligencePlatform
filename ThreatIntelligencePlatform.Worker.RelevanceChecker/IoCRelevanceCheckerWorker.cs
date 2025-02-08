using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.Utils;

namespace ThreatIntelligencePlatform.Worker.RelevanceChecker.Service;

public class IoCRelevanceCheckerWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<IoCRelevanceCheckerWorker> _logger;

    public IoCRelevanceCheckerWorker(IRabbitMQService rabbitMqService, ILogger<IoCRelevanceCheckerWorker> logger)
    {
        _rabbitMQService = rabbitMqService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<IoCDto>("ioc.normalized.queue", async (ioc) =>
        {
            var formattedIoC = IoCFormatter.Format(ioc);
            try
            {
                if (await IsRelevant(ioc))
                {
                    _rabbitMQService.Publish("ioc.relevant", "ioc.relevant.selected", ioc);
                    _logger.LogInformation("Published relevant IoC: {@FormattedIoC}", formattedIoC);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing IoC: {@FormattedIoC}", formattedIoC);
            }
        });
        
        return Task.CompletedTask;
    }

    private async Task<bool> IsRelevant(IoCDto ioc)
    {
        await Task.Delay(50);
        return true;
    }
}