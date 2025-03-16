using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Business.Interfaces;

public interface IIoCService
{
    Task<IEnumerable<IoCDto>> LoadAsync(long limit, long offset,
        CancellationToken cancellationToken = default);
    Task StoreAsync(IEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default);
    IAsyncEnumerable<IoCDto> StreamLoadAsync(long limit = 100, long offset = 0,
        CancellationToken cancellationToken = default);
    Task StreamStoreAsync(IAsyncEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default);
}