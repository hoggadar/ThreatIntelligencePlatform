using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ThreatIntelligencePlatform.MessageBroker.Interfaces;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Shared.Utils;

namespace ThreatIntelligencePlatform.Worker.Normalizer.Services;

public class IoCNormalizerWorker : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<IoCNormalizerWorker> _logger;

    public IoCNormalizerWorker(IRabbitMQService rabbitMQServiece, ILogger<IoCNormalizerWorker> logger)
    {
        _rabbitMQService = rabbitMQServiece;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<IoCDto>("ioc.raw.queue", (ioc) =>
        {
            try
            {
                var normalizedIoC = NormalizeIoC(ioc);
                _rabbitMQService.Publish("ioc.normalized", "ioc.normalized.processed", normalizedIoC);
                _logger.LogInformation("Normalized and published IoC: {@IoCFormatted}", IoCFormatter.Format(normalizedIoC));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing IoC: {IoCId}", ioc.Id);
            }

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }

    private static IoCDto NormalizeIoC(IoCDto ioc)
    {
        return new IoCDto
        {
            Id = Guid.NewGuid().ToString(),
            Source = ioc.Source,
            FirstSeen = ioc.FirstSeen?.ToUniversalTime(),
            LastSeen = ioc.LastSeen?.ToUniversalTime(),
            Type = ioc.Type.ToLowerInvariant(),
            Value = ioc.Value,
            Tags = NormalizeTags(ioc.Tags),
            AdditionalData = ioc.AdditionalData
        };
    }
    
    private static List<string> NormalizeTags(List<string>? tags)
    {
        if (tags == null) return []; 
        
        return tags?
            .Select(tag => Regex.Replace(tag, "[^a-zA-Z0-9]", "").ToUpperInvariant())
            .Where(tag => !string.IsNullOrEmpty(tag))
            .ToList() ?? [];
    }
}