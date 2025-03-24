using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Business.Interfaces;

public interface IIoCService
{
    Task<IEnumerable<IoCDto>> LoadAsync(long limit, long offset, string? search,
        CancellationToken cancellationToken = default);
    Task StoreAsync(IEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default);
    IAsyncEnumerable<IoCDto> StreamLoadAsync(long limit, long offset, string search,
        CancellationToken cancellationToken = default);
    Task StreamStoreAsync(IAsyncEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default);
}