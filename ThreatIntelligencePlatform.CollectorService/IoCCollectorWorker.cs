using Newtonsoft.Json;
using ThreatIntelligencePlatform.CollectorService.Services;
using ThreatIntelligencePlatform.SharedData.DTOs;
using Formatting = System.Xml.Formatting;

namespace ThreatIntelligencePlatform.CollectorService;

public class IoCCollectorWorker : BackgroundService
{
    private readonly TweetFeedService _tweetFeedService;
    private readonly ThreatFoxService _threatFoxService;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
    private readonly ILogger<IoCCollectorWorker> _logger;

    public IoCCollectorWorker(TweetFeedService tweetFeedService, ThreatFoxService threatFoxService,
        ILogger<IoCCollectorWorker> logger)
    {
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
                var threatFoxData = await _threatFoxService.CollectDataAsync(stoppingToken);
                foreach (var ioc in threatFoxData)
                {
                    _logger.LogInformation("ThreatFox IoC:\n{@IoCFormatted}", FormatIoC(ioc));
                }
                _logger.LogInformation("Successfully collected and logged data from all sources");
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
            Formatting = (Newtonsoft.Json.Formatting)Formatting.Indented,
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