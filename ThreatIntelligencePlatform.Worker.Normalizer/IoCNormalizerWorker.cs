using Newtonsoft.Json;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.SharedData.DTOs;

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
            try
            {
                var normalizedIoC = await NormalizeIoC(ioc);
                _rabbitMQService.Publish("ioc.normalized", "ioc.normalized.processed", normalizedIoC);
                _logger.LogInformation("Normalized and published IoC: {@IoCFormatted}", FormatIoC(normalizedIoC));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing IoC: {IoCId}", ioc.Id);
            }
        });

        return Task.CompletedTask;
    }

    private async Task<IoCDto> NormalizeIoC(IoCDto ioc)
    {
        await Task.Delay(100);
        return ioc;
    }
    
    private string FormatIoC(IoCDto ioc)
    {
        return JsonConvert.SerializeObject(new
        {
            ioc.Id,
            ioc.Source,
            ioc.FirstSeen,
            ioc.LastSeen,
            ioc.Type,
            ioc.Value,
            ioc.Tags,
            AdditionalData = ioc.AdditionalData.Count > 0 ? ioc.AdditionalData : null
        }, Formatting.Indented);
    }
}