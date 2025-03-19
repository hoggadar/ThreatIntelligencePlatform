namespace ThreatIntelligencePlatform.Grpc.Clients.Interfaces;

public interface IIoCGrpcClient
{
    Task<IEnumerable<Shared.DTOs.IoCDto>> LoadAsync(long limit, long offset, string search,
        CancellationToken cancellationToken = default);
    Task StoreAsync(IEnumerable<Shared.DTOs.IoCDto> iocs, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Shared.DTOs.IoCDto> StreamLoadAsync(long limit, long offset, string search,
        CancellationToken cancellationToken = default);
    Task StreamStoreAsync(IAsyncEnumerable<Shared.DTOs.IoCDto> iocs, CancellationToken cancellationToken = default);
}