using Newtonsoft.Json;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.Utils;

namespace ThreatIntelligencePlatform.Worker.Normalizer.Services;

public class IoCNormalizerWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<IoCNormalizerWorker> _logger;

    public IoCNormalizerWorker(IRabbitMQService rabbitMQServiece, ILogger<IoCNormalizerWorker> logger)
    {
        _rabbitMQService = rabbitMQServiece;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<IoCDto>("ioc.raw.queue", async (ioc) =>
        {
            var formattedIoC = IoCFormatter.Format(ioc);
            try
            {
                var normalizedIoC = await NormalizeIoC(ioc);
                _rabbitMQService.Publish("ioc.normalized", "ioc.normalized.processed", normalizedIoC);
                _logger.LogInformation("Published normalized IoC: {@FormattedIoC}", formattedIoC);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing IoC: {@FormattedIoC}", formattedIoC);
            }
        });

        return Task.CompletedTask;
    }

    private async Task<IoCDto> NormalizeIoC(IoCDto ioc)
    {
        await Task.Delay(50);
        return ioc;
    }
}