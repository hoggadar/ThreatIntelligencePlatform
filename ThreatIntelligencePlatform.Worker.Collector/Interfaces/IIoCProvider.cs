using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Worker.Collector.Interfaces;

public interface IIoCProvider
{
    IAsyncEnumerable<IoCDto> CollectIoCsAsync(CancellationToken cancellationToken);
    string SourceName { get; }
}