namespace ThreatIntelligencePlatform.Worker.WhitelistCollector.Interfaces;

public interface IWhitelistProvider
{
    IAsyncEnumerable<string> CollectWhitelistAsync(CancellationToken cancellationToken);
    string SourceName { get; }
}