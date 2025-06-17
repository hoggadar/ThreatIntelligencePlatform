using System.Text.Json;
using Microsoft.Extensions.Logging;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.ManualTester;

public class ManualTester
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<ManualTester> _logger;

    public ManualTester(IRabbitMQService rabbitMQService, ILogger<ManualTester> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    public async Task PublishTestDataAsync()
    {
        var testIoCs = GenerateTestIoCs();
        
        foreach (var ioc in testIoCs)
        {
            _rabbitMQService.Publish("ioc.raw", $"ioc.raw.test", ioc);
            _logger.LogInformation("Published test IoC to queue 'ioc.raw': Type={Type}, Value={Value}, Tags={Tags}", 
                ioc.Type, 
                ioc.Value, 
                string.Join(", ", ioc.Tags));
        }
        
        _logger.LogInformation("Successfully published {Count} test IoCs to queue 'ioc.raw'", testIoCs.Count);
    }

    private List<IoCDto> GenerateTestIoCs()
    {
        var now = DateTime.UtcNow;
        return new List<IoCDto>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Source = "TestSource",
                FirstSeen = now.AddDays(-1),
                LastSeen = now,
                Type = "IP",
                Value = "192.168.1.1",
                Tags = new List<string> { "malware", "botnet" },
                AdditionalData = new Dictionary<string, string>
                {
                    { "country", "US" },
                    { "asn", "AS12345" }
                }
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Source = "TestSource",
                FirstSeen = now.AddDays(-2),
                LastSeen = now,
                Type = "Domain",
                Value = "malicious-domain.com",
                Tags = new List<string> { "phishing", "malware" },
                AdditionalData = new Dictionary<string, string>
                {
                    { "registrar", "TestRegistrar" },
                    { "creation_date", now.AddYears(-1).ToString("yyyy-MM-dd") }
                }
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Source = "TestSource",
                FirstSeen = now.AddDays(-3),
                LastSeen = now,
                Type = "Hash",
                Value = "7c4a8d09ca3762af61e59520943dc26494f8941b",
                Tags = new List<string> { "ransomware", "malware" },
                AdditionalData = new Dictionary<string, string>
                {
                    { "file_type", "exe" },
                    { "file_size", "1024" }
                }
            },
            new()   
            {
                Id = Guid.NewGuid().ToString(),
                Source = "TestSource",
                FirstSeen = now.AddDays(-4),
                LastSeen = now,
                Type = "URL",
                Value = "https://malicious-site.com/payload.exe",
                Tags = new List<string> { "malware", "phishing" },
                AdditionalData = new Dictionary<string, string>
                {
                    { "protocol", "https" },
                    { "path", "/payload.exe" }
                }
            }
        };
    }
}