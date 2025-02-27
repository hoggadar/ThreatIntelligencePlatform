using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Worker.IoCCollector.DTOs;
using ThreatIntelligencePlatform.Worker.IoCCollector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.IoCCollector.Services;

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

    public async IAsyncEnumerable<IoCDto> CollectIoCsAsync(CancellationToken cancellationToken)
    {
        await foreach (var ioc in CollectDataAsync(cancellationToken))
        {
            yield return ioc;
        }
    }

    private async IAsyncEnumerable<IoCDto> CollectDataAsync([EnumeratorCancellation]CancellationToken cancellationToken)
    {
        var uri = "api/v1";
        var payload = new { query = "get_iocs", days = 1 };

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync(uri, payload, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect data from ThreatFox. Status code: {StatusCode}",
                    response.StatusCode);
                yield break;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting data from ThreatFox.");
            yield break;
        }
        catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("ThreatFox data collection was canceled.");
            yield break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while collecting data from ThreatFox.");
            yield break;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        await foreach (var ioc in ParseJsonStreamAsync(stream, cancellationToken))
        {
            yield return ioc;
        }
    }
    
    private async IAsyncEnumerable<IoCDto> ParseJsonStreamAsync(Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var response = await JsonSerializer.DeserializeAsync<ThreatFoxResponseDto>(stream, options, cancellationToken);
    
        if (response?.Data != null)
        {
            foreach (var item in response.Data)
            {
                yield return _mapper.Map<IoCDto>(item);
            }
        }
    }
}