using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector.Services;

public class ThreatFoxService : IIoCProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<ThreatFoxService> _logger;

    public string SourceName => "ThreatFox";

    public ThreatFoxService(IHttpClientFactory httpClientFactory, IMapper mapper, ILogger<ThreatFoxService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ThreatFox");
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
            var uri = "api/v1";
            var payload = new { query = "get_iocs", days = 1 };
            
            var response = await _httpClient.PostAsJsonAsync(uri, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect data from ThreatFox. Status code: {StatusCode}",
                    response.StatusCode);
                return [];
            }
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var root = JsonSerializer.Deserialize<ThreatFoxResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (root?.Data == null || !root.Data.Any())
            {
                _logger.LogWarning("No data received from ThreatFox.");
                return [];
            }

            return _mapper.Map<IEnumerable<IoCDto>>(root.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting data from ThreatFox");
            throw;
        }
    }
}