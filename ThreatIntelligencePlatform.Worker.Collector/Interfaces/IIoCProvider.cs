using ThreatIntelligencePlatform.SharedData.DTOs;

namespace ThreatIntelligencePlatform.Worker.Collector.Interfaces;

public interface IIoCProvider
{
    Task<IEnumerable<IoCDto>> CollectIoCsAsync(CancellationToken cancellationToken);
    string SourceName { get; }
}