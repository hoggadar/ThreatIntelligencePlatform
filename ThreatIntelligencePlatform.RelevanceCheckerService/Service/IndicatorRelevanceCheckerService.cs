namespace ThreatIntelligencePlatform.RelevanceCheckerService.Service;

public class IndicatorRelevanceCheckerService : BackgroundService
{
    private readonly ILogger<IndicatorRelevanceCheckerService> _logger;

    public IndicatorRelevanceCheckerService(ILogger<IndicatorRelevanceCheckerService> logger)
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