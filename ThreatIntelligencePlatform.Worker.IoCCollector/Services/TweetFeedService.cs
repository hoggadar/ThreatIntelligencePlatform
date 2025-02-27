using System.Runtime.CompilerServices;
using System.Text.Json;
using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.SharedData.DTOs.TweetFeed;
using ThreatIntelligencePlatform.Worker.IoCCollector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.IoCCollector.Services;

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

    public async IAsyncEnumerable<IoCDto> CollectIoCsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var endpoints = new[] { "ip", "url", "domain", "sha256", "md5" };

        foreach (var endpoint in endpoints)
        {
            await foreach (var ioc in CollectDataAsync(endpoint, cancellationToken))
            {
                yield return ioc;
            }
        }
    }

    private async IAsyncEnumerable<IoCDto> CollectDataAsync(string endpoint, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var uri = $"v1/week/{endpoint}";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to collect {DataType} data from TweetFeed. Status code: {StatusCode}",
                    endpoint.ToUpper(), response.StatusCode);
                yield break;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting {DataType} data from TweetFeed.", endpoint);
            yield break;
        }
        catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("{DataType} data collection from TweetFeed was canceled.", endpoint);
            yield break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while collecting {DataType} data from TweetFeed.", endpoint);
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

        await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<TweetFeedResponseDto>(stream, options,
                           cancellationToken))
        {
            if (item != null)
            {
                yield return _mapper.Map<IoCDto>(item);
            }
        }
    }
}