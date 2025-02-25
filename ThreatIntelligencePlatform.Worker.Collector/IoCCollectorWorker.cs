using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.Shared.Utils;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector;

public class IoCCollectorWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IEnumerable<IIoCProvider> _ioCProviders;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
    private readonly ILogger<IoCCollectorWorker> _logger;

    public IoCCollectorWorker(IRabbitMQService rabbitMqService, IEnumerable<IIoCProvider> iocProviders,
        ILogger<IoCCollectorWorker> logger)
    {
        _rabbitMQService = rabbitMqService;
        _ioCProviders = iocProviders;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            var tasks = _ioCProviders.Select(provider => CollectAndPublish(provider, stoppingToken));
            await Task.WhenAll(tasks);
        }
    }
    
    private async Task CollectAndPublish(IIoCProvider provider, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var ioc in provider.CollectIoCsAsync(cancellationToken))
            {
                _rabbitMQService.Publish("ioc.raw", $"ioc.raw.{provider.SourceName}", ioc);
                _logger.LogInformation("Published {Source} IoC to RabbitMQ:\n{@IoCFormatted}",
                    provider.SourceName, IoCFormatter.Format(ioc));
            }
            _logger.LogInformation("Successfully collected and published data from {Source}", provider.SourceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing IoCs from {Source}", provider.SourceName);
        }
    }
}