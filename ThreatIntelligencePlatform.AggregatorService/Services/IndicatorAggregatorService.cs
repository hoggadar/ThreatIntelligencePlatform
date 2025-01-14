namespace ThreatIntelligencePlatform.AggregatorService.Services;

public class IndicatorAggregatorService : BackgroundService
{
    private readonly ILogger<IndicatorAggregatorService> _logger;

    public IndicatorAggregatorService(ILogger<IndicatorAggregatorService> logger)
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