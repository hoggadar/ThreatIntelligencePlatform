using ThreatIntelligencePlatform.Worker.WhitelistCollector.Caching;
using ThreatIntelligencePlatform.Worker.WhitelistCollector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.WhitelistCollector;

public class WhitelistCollectorWorker : BackgroundService
{
    private readonly IRedisService _redisService;
    private readonly IEnumerable<IWhitelistProvider> _whitelistProviders;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(60);
    private readonly SemaphoreSlim _semaphore = new(3);
    private readonly ILogger<WhitelistCollectorWorker> _logger;

    public WhitelistCollectorWorker(IRedisService redisService, IEnumerable<IWhitelistProvider> whitelistProviders,
        ILogger<WhitelistCollectorWorker> logger)
    {
        _redisService = redisService;
        _whitelistProviders = whitelistProviders;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_interval);

        do
        {
            _logger.LogInformation("Starting whitelist collection...");

            var tasks = _whitelistProviders.Select(provider => ProcessProviderSafely(provider, stoppingToken));
            await Task.WhenAll(tasks);

            _logger.LogInformation("Completed whitelist collection.");
        } while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
    }
    
    private async Task ProcessProviderSafely(IWhitelistProvider provider, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Starting collection from {Source}", provider.SourceName);

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(30));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            await CollectAndStore(provider, linkedCts.Token);
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
            _logger.LogError(ex, "An unexpected error occurred while collecting data from {Source}", provider.SourceName);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task CollectAndStore(IWhitelistProvider provider, CancellationToken cancellationToken)
    {
        await foreach (var domain in provider.CollectWhitelistAsync(cancellationToken))
        {
            var key = $"{provider.SourceName}:{domain}";

            try
            {
                await _redisService.SetAsync(key, domain, TimeSpan.FromDays(5));
                _logger.LogInformation("Saved to Redis: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save {Key} to Redis", key);
            }
        }

        _logger.LogInformation("Successfully collected data from {Source}", provider.SourceName);
    }
}