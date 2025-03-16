using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Grpc.Clients.Interfaces;
using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Business.Services;

public class IoCService : IIoCService
{
    private readonly IIoCGrpcClient _grpcClient;

    public IoCService(IIoCGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    
    public async Task<IEnumerable<IoCDto>> LoadAsync(long limit, long offset,
        CancellationToken cancellationToken = default)
    {
        return await _grpcClient.LoadAsync(limit, offset, cancellationToken);
    }

    public async Task StoreAsync(IEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default)
    {
        await _grpcClient.StoreAsync(iocs, cancellationToken);
    }

    public IAsyncEnumerable<IoCDto> StreamLoadAsync(long limit = 100, long offset = 0,
        CancellationToken cancellationToken = default)
    {
        return _grpcClient.StreamLoadAsync(limit, offset, cancellationToken);
    }

    public async Task StreamStoreAsync(IAsyncEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default)
    {
        await _grpcClient.StreamStoreAsync(iocs, cancellationToken);
    }
}