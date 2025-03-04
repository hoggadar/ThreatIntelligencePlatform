using Serilog;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.Shared.Caching;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Shared.Utils;

namespace ThreatIntelligencePlatform.Worker.RelevanceChecker;

public class IoCRelevanceCheckerWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IRedisService _redisService;
    private readonly ILogger<IoCRelevanceCheckerWorker> _logger;

    public IoCRelevanceCheckerWorker(IRabbitMQService rabbitMqService, IRedisService redisService,
        ILogger<IoCRelevanceCheckerWorker> logger)
    {
        _rabbitMQService = rabbitMqService;
        _redisService = redisService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<IoCDto>("ioc.normalized.queue", async (ioc) =>
        {
            var formattedIoC = IoCFormatter.Format(ioc);
            try
            {
                var isRelevant = await IsRelevant(ioc);
                if (isRelevant)
                {
                    _rabbitMQService.Publish("ioc.relevant", "ioc.relevant.selected", ioc);
                    Log.Information("Published relevant IoC: {@FormattedIoC}", formattedIoC);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing IoC: {@FormattedIoC}", formattedIoC);
            }
        });
        
        return Task.CompletedTask;
    }

    private async Task<bool> IsRelevant(IoCDto ioc)
    {
        var key = $"{ioc.Source}:{ioc.Value}";
        var exists = await _redisService.ExistsAsync(key);
        
        if (!exists)
        {
            _logger.LogInformation("IoC not found in whitelist: {Key}", key);
            return true;
        }
        
        _logger.LogInformation("IoC found in whitelist: {Key}", key);
        return false;
    }
}