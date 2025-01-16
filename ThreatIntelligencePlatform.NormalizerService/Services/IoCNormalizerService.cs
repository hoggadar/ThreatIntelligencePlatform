namespace ThreatIntelligencePlatform.NormalizerService.Services;

public class IoCNormalizerService : BackgroundService
{
    private readonly ILogger<IoCNormalizerService> _logger;

    public IoCNormalizerService(ILogger<IoCNormalizerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}