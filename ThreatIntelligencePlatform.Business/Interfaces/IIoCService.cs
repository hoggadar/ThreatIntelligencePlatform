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
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountByTypeAsync(CancellationToken cancellationToken = default);
    Task<long> CountSpecificTypeAsync(string type, CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountBySourceAsync(string source, CancellationToken cancellationToken = default);
    Task<long> CountSpecificSourceAsync(string source, CancellationToken cancellationToken = default);
    Task<Dictionary<string, Dictionary<string, long>>> CountTypesBySourceAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountBySourceAndTypeAsync(string source, string type, CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountByTypeAndSourceAsync(string type, string source, CancellationToken cancellationToken = default);
}