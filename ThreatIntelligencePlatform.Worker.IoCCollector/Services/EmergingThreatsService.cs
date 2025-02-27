using System.Runtime.CompilerServices;
using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.IoCCollector.DTOs;
using ThreatIntelligencePlatform.Worker.IoCCollector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.IoCCollector.Services;

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

    public async IAsyncEnumerable<IoCDto> CollectIoCsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var ioc in CollectDataAsync(cancellationToken))
        {
            yield return ioc;
        }
    }

    private async IAsyncEnumerable<IoCDto> CollectDataAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var uri = "blockrules/compromised-ips.txt";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect data from EmergingThreats. Status code: {StatusCode}", response.StatusCode);
                yield break;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting data from EmergingThreats.");
            yield break;
        }
        catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("EmergingThreats data collection was canceled.");
            yield break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while collecting data from EmergingThreats.");
            yield break;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line)) continue;

            var dto = new EmergingThreatsResponseDto { IoC = line };
            yield return _mapper.Map<IoCDto>(dto);
        }
    }
}