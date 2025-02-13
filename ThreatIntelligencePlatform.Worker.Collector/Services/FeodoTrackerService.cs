using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector.Services;

public class FeodoTrackerService : IIoCProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<FeodoTrackerService> _logger;
    
    public string SourceName => "FeodoTracker";

    public FeodoTrackerService(IHttpClientFactory httpClientFactory, IMapper mapper,
        ILogger<FeodoTrackerService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<IoCDto>> CollectIoCsAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task<IEnumerable<IoCDto>>>
        {
            CollectDataAsync(cancellationToken),
        };
        var data = await Task.WhenAll(tasks);
        return data.SelectMany(x => x);
    }

    private async Task<IEnumerable<IoCDto>> CollectDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var uri = "downloads/ipblocklist_aggressive.txt";
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect  data from TweetFeed. Status code: {StatusCode}",
                    response.StatusCode);
                return [];
            }
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var data = content.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting data from FeodoTracker");
            throw;
        }
    }
}