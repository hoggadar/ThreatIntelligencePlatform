namespace ThreatIntelligencePlatform.Grpc.Clients.Interfaces;

public interface IIoCGrpcClient
{
    Task<IEnumerable<Shared.DTOs.IoCDto>> LoadAsync(long limit, long offset,
        CancellationToken cancellationToken = default);
    Task StoreAsync(IEnumerable<Shared.DTOs.IoCDto> iocs, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Shared.DTOs.IoCDto> StreamLoadAsync(long limit = 100, long offset = 0,
        CancellationToken cancellationToken = default);
    Task StreamStoreAsync(IAsyncEnumerable<Shared.DTOs.IoCDto> iocs, CancellationToken cancellationToken = default);
}