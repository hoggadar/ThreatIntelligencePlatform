using System.Runtime.CompilerServices;
using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.Collector.DTOs;
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
        _httpClient = httpClientFactory.CreateClient("FeodoTracker");
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
        var uri = "downloads/ipblocklist_aggressive.txt";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect data from FeodoTracker. Status code: {StatusCode}",
                    response.StatusCode);
                yield break;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting data from FeodoTracker.");
            yield break;
        }
        catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("FeodoTracker data collection was canceled.");
            yield break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while collecting data from FeodoTracker.");
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

            var dto = new FeodoTrackerResponseDto { IoC = line };
            yield return _mapper.Map<IoCDto>(dto);
        }
    }
}