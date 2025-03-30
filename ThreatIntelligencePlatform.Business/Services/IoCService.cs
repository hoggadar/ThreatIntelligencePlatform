using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Grpc.Clients;
using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Business.Services;

public class IoCService : IIoCService
{
    private readonly IIoCGrpcClient _grpcClient;

    public IoCService(IIoCGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    
    public async Task<IEnumerable<IoCDto>> LoadAsync(long limit, long offset, string? search,
        CancellationToken cancellationToken = default)
    {
        return await _grpcClient.LoadAsync(limit, offset, search, cancellationToken);
    }

    public async Task StoreAsync(IEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default)
    {
        await _grpcClient.StoreAsync(iocs, cancellationToken);
    }

    public IAsyncEnumerable<IoCDto> StreamLoadAsync(long limit, long offset, string search,
        CancellationToken cancellationToken = default)
    {
        return _grpcClient.StreamLoadAsync(limit, offset, search, cancellationToken);
    }

    public async Task StreamStoreAsync(IAsyncEnumerable<IoCDto> iocs, CancellationToken cancellationToken = default)
    {
        await _grpcClient.StreamStoreAsync(iocs, cancellationToken);
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountAsync(cancellationToken);
    }

    public async Task<Dictionary<string, long>> CountByTypeAsync(CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountByTypeAsync(cancellationToken);
    }

    public async Task<long> CountSpecificTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountSpecificTypeAsync(type, cancellationToken);
    }

    public async Task<Dictionary<string, long>> CountBySourceAsync(
        CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountBySourceAsync(cancellationToken);
    }

    public async Task<long> CountSpecificSourceAsync(string source, CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountSpecificSourceAsync(source, cancellationToken);
    }

    public async Task<Dictionary<string, Dictionary<string, long>>> CountTypesBySourceAsync(
        CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountTypesBySourceAsync(cancellationToken);
    }

    public async Task<Dictionary<string, long>> CountBySourceAndTypeAsync(string source,
        CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountBySourceAndTypeAsync(source, cancellationToken);
    }

    public async Task<Dictionary<string, long>> CountByTypeAndSourceAsync(string type,
        CancellationToken cancellationToken = default)
    {
        return await _grpcClient.CountByTypeAndSourceAsync(type, cancellationToken);
    }
}