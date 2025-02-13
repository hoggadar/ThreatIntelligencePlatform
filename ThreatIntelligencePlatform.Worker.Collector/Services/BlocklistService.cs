using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.Collector.Services;

public class BlocklistService : IIoCProvider
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<BlocklistService> _logger;
    
    public string SourceName => "Blocklist";

    public BlocklistService(IHttpClientFactory httpClientFactory, IMapper mapper, ILogger<BlocklistService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Blocklist");
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
            var uri = "lists/all.txt";
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect data from Blocklist.de. Status code: {StatusCode}",
                    response.StatusCode);
                return [];
            }
            
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            
            var data = new List<BlocklistResponse>();
            string? line;

            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    data.Add(new BlocklistResponse { IoC = line});
                }
            }

            if (data.Count == 0)
            {
                _logger.LogWarning("No data received from Blocklist");
                return [];
            }
            
            return _mapper.Map<IEnumerable<IoCDto>>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting data from Blocklist");
            throw;
        }
    }
}