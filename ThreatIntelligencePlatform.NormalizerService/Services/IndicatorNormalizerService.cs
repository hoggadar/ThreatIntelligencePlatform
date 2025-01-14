namespace ThreatIntelligencePlatform.NormalizerService.Services;

public class IndicatorNormalizerService : BackgroundService
{
    private readonly ILogger<IndicatorNormalizerService> _logger;

    public IndicatorNormalizerService(ILogger<IndicatorNormalizerService> logger)
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