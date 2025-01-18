using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.SharedData.DTOs;

namespace ThreatIntelligencePlatform.NormalizerService.Services;

public class IoCNormalizerWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<IoCNormalizerWorker> _logger;

    public IoCNormalizerWorker(IRabbitMQService rabbitMQServiece, ILogger<IoCNormalizerWorker> logger)
    {
        _rabbitMQService = rabbitMQServiece;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<IoCDto>("raw_ioc_queue", async (ioc) =>
        {
            try
            {
                var normalizedIoC = await NormalizeIoC(ioc);
                _rabbitMQService.Publish("ioc_exchange", "normalized_ioc", normalizedIoC);
                _logger.LogInformation("Normalized and published IoC: {IoCId}", ioc.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing IoC: {IoCId}", ioc.Id);
            }
        });
    }

    private async Task<IoCDto> NormalizeIoC(IoCDto ioc)
    {
        return ioc;
    }
}