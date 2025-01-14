using ThreatIntelligencePlatform.SharedData.DTOs;

namespace ThreatIntelligencePlatform.CollectorService.Services;

public class TweetFeedService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ThreatFoxService> _logger;

    public TweetFeedService(IHttpClientFactory httpClientFactory, ILogger<ThreatFoxService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }
}