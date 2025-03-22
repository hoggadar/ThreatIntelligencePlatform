using ThreatIntelligencePlatform.Shared.Caching;
using ThreatIntelligencePlatform.Worker.WhitelistCollector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.WhitelistCollector;

public class WhitelistCollectorWorker : BackgroundService
{
    private readonly IRedisService _redisService;
    private readonly List<IWhitelistProvider> _whitelistProviders;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(60);
    private readonly SemaphoreSlim _semaphore = new(3);
    private readonly ILogger<WhitelistCollectorWorker> _logger;

    public WhitelistCollectorWorker(IRedisService redisService, IEnumerable<IWhitelistProvider> whitelistProviders,
        ILogger<WhitelistCollectorWorker> logger)
    {
        _redisService = redisService;
        _whitelistProviders = whitelistProviders.ToList();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_interval);

        do
        {
            _logger.LogInformation("Starting whitelist collection");

            var tasks = _whitelistProviders.Select(provider => ProcessProviderSafely(provider, stoppingToken));
            await Task.WhenAll(tasks);

            _logger.LogInformation("Completed whitelist collection");
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
            
            _logger.LogInformation("Successfully completed collection from {Source}", provider.SourceName);
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
        const int batchSize = 1000;
        var currentBatch = new List<string>(batchSize);
        var processedCount = 0;
        
        try
        {
            await foreach (var domain in provider.CollectWhitelistAsync(cancellationToken))
            {
                currentBatch.Add(domain);
                
                if (currentBatch.Count >= batchSize)
                {
                    await ProcessDomainBatch(provider.SourceName, currentBatch, cancellationToken);
                    processedCount += currentBatch.Count;
                    _logger.LogInformation("Progress update for {Source}: processed {ProcessedCount} domains", 
                        provider.SourceName, processedCount);
                    
                    currentBatch = new List<string>(batchSize);
                }
            }
            
            if (currentBatch.Any())
            {
                await ProcessDomainBatch(provider.SourceName, currentBatch, cancellationToken);
                processedCount += currentBatch.Count;
            }
            
            _logger.LogInformation("Successfully completed processing whitelist from {Source}. Total domains processed: {ProcessedCount}", 
                provider.SourceName, processedCount);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Processing cancelled for {Source} after processing {ProcessedCount} domains", 
                provider.SourceName, processedCount);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing whitelist from {Source} after processing {ProcessedCount} domains", 
                provider.SourceName, processedCount);
            throw;
        }
    }

    private async Task ProcessDomainBatch(string source, List<string> domains, CancellationToken cancellationToken)
    {
        try
        {
            await _redisService.AddToWhitelistBatchAsync(source, domains, TimeSpan.FromDays(5));
            _logger.LogDebug("Successfully processed batch of {Count} domains from {Source}", domains.Count, source);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch of {Count} domains from {Source}", domains.Count, source);
            throw;
        }
    }
}