using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.Shared.Utils;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector;

public class IoCCollectorWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IEnumerable<IIoCProvider> _ioCProviders;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);
    private readonly SemaphoreSlim _semaphore = new(3);
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
            _logger.LogInformation("Starting IoC collection cycle...");

            var tasks = _ioCProviders.Select(provider => ProcessProviderSafely(provider, stoppingToken));

            await Task.WhenAll(tasks);

            _logger.LogInformation("Completed IoC collection cycle.");
        }
    }

    private async Task ProcessProviderSafely(IIoCProvider provider, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Starting collection from {Source}", provider.SourceName);

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            await CollectAndPublish(provider, linkedCts.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Cancellation requested, stopping collection from {Source}", provider.SourceName);
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("Timeout reached while collecting data from {Source}", provider.SourceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while collecting data from {Source}",
                provider.SourceName);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task CollectAndPublish(IIoCProvider provider, CancellationToken cancellationToken)
    {
        await foreach (var ioc in provider.CollectIoCsAsync(cancellationToken))
        {
            _rabbitMQService.Publish("ioc.raw", $"ioc.raw.{provider.SourceName}", ioc);
            _logger.LogInformation("Published {Source} IoC to RabbitMQ:\n{@IoCFormatted}",
                provider.SourceName, IoCFormatter.Format(ioc));
        }

        _logger.LogInformation("Successfully collected and published data from {Source}", provider.SourceName);
    }
}