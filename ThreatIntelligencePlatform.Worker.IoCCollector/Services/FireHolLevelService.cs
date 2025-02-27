using System.Runtime.CompilerServices;
using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.IoCCollector.DTOs;
using ThreatIntelligencePlatform.Worker.IoCCollector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.IoCCollector.Services;

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

    public async IAsyncEnumerable<IoCDto> CollectIoCsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var ioc in CollectDataAsync(cancellationToken))
        {
            yield return ioc;
        }
    }

    private async IAsyncEnumerable<IoCDto> CollectDataAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var uri = "firehol/blocklist-ipsets/master/firehol_level1.netset";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect data from FireHolLevel. Status code: {StatusCode}",
                    response.StatusCode);
                yield break;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting data from FireHolLevel.");
            yield break;
        }
        catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("FireHolLevel data collection was canceled.");
            yield break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while collecting data from FireHolLevel.");
            yield break;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null) continue;

            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

            var dto = new FireHolLevelResponseDto { IoC = line };
            yield return _mapper.Map<IoCDto>(dto);
        }
    }
}