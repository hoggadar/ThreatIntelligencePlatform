using System.Runtime.CompilerServices;
using System.Text;
using ThreatIntelligencePlatform.Worker.WhitelistCollector.Interfaces;

namespace ThreatIntelligencePlatform.Worker.WhitelistCollector.Services;

public class MajesticService : IWhitelistProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MajesticService> _logger;
    
    public string SourceName => "MajesticMillion";

    public MajesticService(IHttpClientFactory httpClientFactory, ILogger<MajesticService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("MajesticMillion");
        _httpClient.Timeout = TimeSpan.FromMinutes(5);
        _logger = logger;
    }

    public async IAsyncEnumerable<string> CollectWhitelistAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var item in CollectDataAsync(cancellationToken))
        {
            yield return item;
        }
    }

    private async IAsyncEnumerable<string> CollectDataAsync([EnumeratorCancellation]CancellationToken cancellationToken)
    {
        var uri = "majestic_million.csv";
        
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch {Uri}. Status code: {StatusCode}", uri, response.StatusCode);
                yield break;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting {Uri}", uri);
            yield break;
        }
        catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Whitelist data collection was canceled.");
            yield break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while collecting data from {Uri}", uri);
            yield break;
        }
        
        response = await _httpClient.GetAsync(uri, cancellationToken);

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        await reader.ReadLineAsync(cancellationToken);

        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = line.Split(',');
            if (fields.Length < 3) continue;
            
            yield return fields[2].Trim().ToLower();
        }
    }
}