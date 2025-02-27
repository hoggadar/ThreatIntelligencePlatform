using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Worker.IoCCollector.Interfaces;

public interface IIoCProvider
{
    IAsyncEnumerable<IoCDto> CollectIoCsAsync(CancellationToken cancellationToken);
    string SourceName { get; }
}