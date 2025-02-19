using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector.Services;

public class EmergingThreatsService : IIoCProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<EmergingThreatsService> _logger;
    
    public string SourceName => "EmergingThreats";

    public EmergingThreatsService(IHttpClientFactory httpClientFactory, IMapper mapper,
        ILogger<EmergingThreatsService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("EmergingThreats");
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
            var uri = "blockrules/compromised-ips.txt";
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect  data from EmergingThreats. Status code: {StatusCode}",
                    response.StatusCode);
                return [];
            }
            
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            
            var data = new List<EmergingThreatsResponseDto>();
            string? line;

            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                if (!string.IsNullOrWhiteSpace(line)) continue;
                data.Add(new EmergingThreatsResponseDto { IoC = line});
            }
            
            if (data.Count == 0)
            {
                _logger.LogWarning("No data received from EmergingThreats");
                return [];
            }

            return _mapper.Map<IEnumerable<IoCDto>>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting data from EmergingThreats");
            throw;
        }
    }
}