using System.Net.Http.Json;
using System.Text.Json;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.DTOs.ThreatFox;

namespace ThreatIntelligencePlatform.CollectorService.Services;

public class ThreatFoxService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ThreatFoxService> _logger;

    public ThreatFoxService(IHttpClientFactory httpClientFactory, ILogger<ThreatFoxService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<IoCDto>> CollectDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ThreatFox");
            var uri = "api/v1";
            var payload = new { query = "get_iocs", days = 1 };
            
            var response = await httpClient.PostAsJsonAsync(uri, payload, cancellationToken);
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
            
            var iocDtos = root.Data.Select(item => new IoCDto
            {
                Id = item.Id,
                Source = "ThreatFox",
                FirstSeen = ParseDateTime(item.FirstSeen),
                LastSeen = ParseDateTime(item.LastSeen),
                Type = item.IocType,
                Value = item.Ioc,
                Tags = item.Tags,
                AdditionalData = new Dictionary<string, string>
                {
                    ["threat_type"] = item.ThreatType,
                    ["threat_type_desc"] = item.ThreatTypeDesc,
                    ["confidence_level"] = item.ConfidenceLevel.ToString()
                }
            });

            return iocDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting data from ThreatFox");
            throw;
        }
    }
    
    private DateTime? ParseDateTime(string? dateTimeStr)
    {
        return DateTime.TryParse(dateTimeStr, out var dateTime) ? dateTime : null;
    }
}