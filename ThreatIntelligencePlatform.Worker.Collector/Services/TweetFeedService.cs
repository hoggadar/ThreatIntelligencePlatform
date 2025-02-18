using System.Text.Json;
using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.SharedData.DTOs.TweetFeed;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector.Services;

public class TweetFeedService : IIoCProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<ThreatFoxService> _logger;
    
    public string SourceName => "TweetFeed";
    
    public TweetFeedService(IHttpClientFactory httpClientFactory, IMapper mapper, ILogger<ThreatFoxService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("TweetFeed");
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<IoCDto>> CollectIoCsAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task<IEnumerable<IoCDto>>>
        {
            CollectDataAsync("ip", cancellationToken),
            CollectDataAsync("url", cancellationToken),
            CollectDataAsync("domain", cancellationToken),
            CollectDataAsync("sha256", cancellationToken),
            CollectDataAsync("md5", cancellationToken)
        };
        var data = await Task.WhenAll(tasks);
        return data.SelectMany(x => x);
    }

    private async Task<IEnumerable<IoCDto>> CollectDataAsync(string endpoint, CancellationToken cancellationToken)
    {
        try
        {
            var uri = $"v1/week/{endpoint}";
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect {DataType} data from TweetFeed. Status code: {StatusCode}",
                    endpoint.ToUpper(), response.StatusCode);
                return [];
            }
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = JsonSerializer.Deserialize<List<TweetFeedResponseDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null || data.Count == 0)
            {
                _logger.LogWarning("No {DataType} data received from TweetFeed", endpoint);
                return [];
            }

            return _mapper.Map<IEnumerable<IoCDto>>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting {DataType} data from TweetFeed", endpoint);
            throw;
        }
    }
}