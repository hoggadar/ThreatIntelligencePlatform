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
                    _logger.LogInformation("Published relevant IoC: {@FormattedIoC}", formattedIoC);
                }
                else
                {
                    _logger.LogInformation("Skipped whitelisted IoC: {@FormattedIoC}", formattedIoC);
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
        try
        {
            var exists = await _redisService.IsInWhitelistAsync(ioc.Source, ioc.Value);
            
            if (!exists)
            {
                _logger.LogDebug("IoC not found in whitelist: {Source}:{Value}", ioc.Source, ioc.Value);
                return true;
            }
            
            _logger.LogDebug("IoC found in whitelist: {Source}:{Value}", ioc.Source, ioc.Value);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking whitelist for IoC: {Source}:{Value}", ioc.Source, ioc.Value);
            return true;
        }
    }
}