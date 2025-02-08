using Newtonsoft.Json;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.Utils;
using ThreatIntelligencePlatform.Worker.Collector.Services;

namespace ThreatIntelligencePlatform.Worker.Collector;

public class IoCCollectorWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly TweetFeedService _tweetFeedService;
    private readonly ThreatFoxService _threatFoxService;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(15);
    private readonly ILogger<IoCCollectorWorker> _logger;

    public IoCCollectorWorker(IRabbitMQService rabbitMqService, TweetFeedService tweetFeedService,
        ThreatFoxService threatFoxService, ILogger<IoCCollectorWorker> logger)
    {
        _rabbitMQService = rabbitMqService;
        _tweetFeedService = tweetFeedService;
        _threatFoxService = threatFoxService;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_interval);

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tweetFeedData = await _tweetFeedService.CollectDataAsync(stoppingToken);
                foreach (var ioc in tweetFeedData)
                {
                    _rabbitMQService.Publish("ioc.raw", "ioc.raw.tweetfeed", ioc);
                    _logger.LogInformation("Published raw IoC | TweetFeed:\n{@FormattedIoC}",
                        IoCFormatter.Format(ioc));
                }
                
                var threatFoxData = await _threatFoxService.CollectDataAsync(stoppingToken);
                foreach (var ioc in threatFoxData)
                {
                    _rabbitMQService.Publish("ioc.raw", "ioc.raw.threatfox", ioc);
                    _logger.LogInformation("Published raw IoC | ThreatFox:\n{@FormattedIoC}",
                        IoCFormatter.Format(ioc));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during data collection");
            }
        }
    }
}