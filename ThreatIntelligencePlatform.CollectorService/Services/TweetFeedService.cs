using System.Text.Json;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.DTOs.TweetFeed;
using ThreatIntelligencePlatform.SharedData.Enums;
using ThreatIntelligencePlatform.SharedData.Utils;

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

    public async Task<IEnumerable<IoCDto>> CollectDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("TweetFeed");
            var uri = "v1/week/ip";
            
            var response = await httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect data from TweetFeed. Status code: {StatusCode}",
                    response.StatusCode);
                return [];
            }
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var root = JsonSerializer.Deserialize<List<TweetFeedResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (root == null || !root.Any())
            {
                _logger.LogWarning("No data received from TweetFeed.");
                return [];
            }

            var iocDtos = root.Select(item => new IoCDto
            {
                Id = null,
                Source = SourceName.TweetFeed.ToString(),
                FirstSeen = DateTimeParser.Parse(item.Date),
                LastSeen = null,
                Type = item.Type,
                Value = item.Value,
                Tags = item.Tags,
                AdditionalData = new Dictionary<string, string>
                {
                    ["user"] = item.User,
                    ["tweet"] = item.Tweet,
                }
            });
            
            return iocDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting data from TweetFeed");
            throw;
        }
    }
}