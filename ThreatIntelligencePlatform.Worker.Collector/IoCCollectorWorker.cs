using Newtonsoft.Json;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.Services;

namespace ThreatIntelligencePlatform.Worker.Collector;

public class IoCCollectorWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly TweetFeedService _tweetFeedService;
    private readonly ThreatFoxService _threatFoxService;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
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
                    _logger.LogInformation("Published TweetFeed IoC to RabbitMQ:\n{@IoCFormatted}", FormatIoC(ioc));
                }
                
                var threatFoxData = await _threatFoxService.CollectDataAsync(stoppingToken);
                foreach (var ioc in threatFoxData)
                {
                    _rabbitMQService.Publish("ioc.raw", "ioc.raw.threatfox", ioc);
                    _logger.LogInformation("Published ThreatFox IoC to RabbitMQ:\n{@IoCFormatted}", FormatIoC(ioc));
                }
                
                _logger.LogInformation("Successfully collected and published data from all sources");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during data collection");
            }
        }
    }

    private string FormatIoC(IoCDto ioc)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        return JsonConvert.SerializeObject(new
        {
            ioc.Id,
            ioc.Source,
            ioc.FirstSeen,
            ioc.LastSeen,
            ioc.Type,
            ioc.Value,
            ioc.Tags,
            AdditionalData = ioc.AdditionalData.Count > 0 ? ioc.AdditionalData : null
        }, settings);
    }
}