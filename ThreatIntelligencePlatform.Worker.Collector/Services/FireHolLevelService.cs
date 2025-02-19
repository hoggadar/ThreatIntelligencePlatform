using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector.Services;

public class FireHolLevelService : IIoCProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<FireHolLevelService> _logger;
    
    public string SourceName => "FireHolLevel";

    public FireHolLevelService(IHttpClientFactory httpClientFactory, IMapper mapper, ILogger<FireHolLevelService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("FireHolLevel");
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
            var uri = "firehol/blocklist-ipsets/master/firehol_level1.netset";
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect  data from FireHolLevel. Status code: {StatusCode}",
                    response.StatusCode);
                return [];
            }
            
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            var data = new List<FireHolLevelResponseDto>();
            string? line;
            
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                data.Add(new FireHolLevelResponseDto { IoC = line});
            }

            if (data.Count == 0)
            {
                _logger.LogWarning("No data received from FireHolLevel");
                return [];
            }
            
            return _mapper.Map<IEnumerable<IoCDto>>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting data from FireHolLevel");
            throw;
        }
    }
}