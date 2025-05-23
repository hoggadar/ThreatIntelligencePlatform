﻿using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Ioc;

namespace ThreatIntelligencePlatform.Grpc.Clients;

public interface IIoCGrpcClient
{
    Task<IEnumerable<Shared.DTOs.IoCDto>> LoadAsync(long limit, long offset, string? search,
        CancellationToken cancellationToken = default);
    Task StoreAsync(IEnumerable<Shared.DTOs.IoCDto> iocs, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Shared.DTOs.IoCDto> StreamLoadAsync(long limit, long offset, string search,
        CancellationToken cancellationToken = default);
    Task StreamStoreAsync(IAsyncEnumerable<Shared.DTOs.IoCDto> iocs, CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountByTypeAsync(CancellationToken cancellationToken = default);
    Task<long> CountSpecificTypeAsync(string type, CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountBySourceAsync(CancellationToken cancellationToken = default);
    Task<long> CountSpecificSourceAsync(string source, CancellationToken cancellationToken = default);
    Task<Dictionary<string, Dictionary<string, long>>> CountTypesBySourceAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountBySourceAndTypeAsync(string source, CancellationToken cancellationToken = default);
    Task<Dictionary<string, long>> CountByTypeAndSourceAsync(string type, CancellationToken cancellationToken = default);
}

public class IoCGrpcClient : IIoCGrpcClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly Database.DatabaseClient _client;
    private bool _disposed;

    public IoCGrpcClient(string grpcServiceUrl)
    {
        _channel = GrpcChannel.ForAddress(grpcServiceUrl);
        _client = new Database.DatabaseClient(_channel);
    }

    public async Task<IEnumerable<Shared.DTOs.IoCDto>> LoadAsync(long limit, long offset, string? search,
        CancellationToken cancellationToken = default)
    {
        LoadRequest request;
        if (search is null)
        {
            request = new LoadRequest
            {
                Limit = limit,
                Offset = offset,
            };
        }
        else
        {
            request = new LoadRequest
            {
                Limit = limit,
                Offset = offset,
                Filter = search
            };
        }
        

        var response = await _client.LoadAsync(request, cancellationToken: cancellationToken);
        return response.IoCs.Select(MapToDto);
    }

    public async Task StoreAsync(IEnumerable<Shared.DTOs.IoCDto> iocs, CancellationToken cancellationToken = default)
    {
        var request = new StoreRequest();
        foreach (var ioc in iocs)
        {
            request.IoCs.Add(MapToProto(ioc));
        }

        await _client.StoreAsync(request, cancellationToken: cancellationToken);
    }

    public async IAsyncEnumerable<Shared.DTOs.IoCDto> StreamLoadAsync(long limit , long offset, string search,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new LoadRequest
        {
            Limit = limit,
            Offset = offset,
            Filter = search,
        };

        using var call = _client.StreamLoad(request, cancellationToken: cancellationToken);
        
        await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
        {
            yield return MapToDto(response.Ioc);
        }
    }

    public async Task StreamStoreAsync(IAsyncEnumerable<Shared.DTOs.IoCDto> iocs,
        CancellationToken cancellationToken = default)
    {
        using var call = _client.StreamStore(cancellationToken: cancellationToken);
        
        await foreach (var ioc in iocs.WithCancellation(cancellationToken))
        {
            await call.RequestStream.WriteAsync(new StreamStoreRequest
            {
                Ioc = MapToProto(ioc)
            }, cancellationToken);
        }
        
        await call.RequestStream.CompleteAsync();
        await call;
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.CountAsync(new Empty(), cancellationToken: cancellationToken);
        return response.Count;
    }

    public async Task<Dictionary<string, long>> CountByTypeAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.CountByTypeAsync(new Empty(), cancellationToken: cancellationToken);
        return response.TypeCounts.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public async Task<long> CountSpecificTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        var request = new CountSpecificTypeRequest { Type = type };
        var response = await _client.CountSpecificTypeAsync(request, cancellationToken: cancellationToken);
        return response.Count;
    }
    
    public async Task<Dictionary<string, long>> CountBySourceAsync(CancellationToken cancellationToken = default)
    {
        var request = new CountBySourceRequest();
        var response = await _client.CountBySourceAsync(request, cancellationToken: cancellationToken);
        return response.SourceCounts.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public async Task<long> CountSpecificSourceAsync(string source, CancellationToken cancellationToken = default)
    {
        var request = new CountSpecificSourceRequest { Source = source };
        var response = await _client.CountSpecificSourceAsync(request, cancellationToken: cancellationToken);
        return response.Count;
    }
    
    public async Task<Dictionary<string, Dictionary<string, long>>> CountTypesBySourceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.CountTypesBySourceAsync(new Empty(), cancellationToken: cancellationToken);
        return response.SourceTypeCounts.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.TypeCounts.ToDictionary(tc => tc.Key, tc => tc.Value)
        );
    }
    
    public async Task<Dictionary<string, long>> CountBySourceAndTypeAsync(string source, CancellationToken cancellationToken = default)
    {
        var request = new CountBySourceAndTypeRequest { Source = source };
        var response = await _client.CountBySourceAndTypeAsync(request, cancellationToken: cancellationToken);
        return response.TypeCounts.ToDictionary(kv => kv.Key, kv => kv.Value);
    }
    
    public async Task<Dictionary<string, long>> CountByTypeAndSourceAsync(string type, CancellationToken cancellationToken = default)
    {
        var request = new CountByTypeAndSourceRequest { Type = type };
        var response = await _client.CountByTypeAndSourceAsync(request, cancellationToken: cancellationToken);
        return response.SourceCounts.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private static Shared.DTOs.IoCDto MapToDto(IoCDto protoDto)
    {
        return new Shared.DTOs.IoCDto
        {
            Id = protoDto.Id,
            Source = protoDto.Source,
            FirstSeen = protoDto.FirstSeen != null ? protoDto.FirstSeen.ToDateTime() : null,
            LastSeen = protoDto.LastSeen != null ? protoDto.LastSeen.ToDateTime() : null,
            Type = protoDto.Type,
            Value = protoDto.Value,
            Tags = protoDto.Tags.ToList(),
            AdditionalData = protoDto.AdditionalData.ToDictionary(kv => kv.Key, kv => kv.Value)
        };
    }

    private static IoCDto MapToProto(Shared.DTOs.IoCDto dto)
    {
        var protoDto = new IoCDto
        {
            Id = dto.Id ?? string.Empty,
            Source = dto.Source,
            Type = dto.Type,
            Value = dto.Value
        };

        if (dto.FirstSeen.HasValue)
            protoDto.FirstSeen = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.FirstSeen.Value, DateTimeKind.Utc));

        if (dto.LastSeen.HasValue)
            protoDto.LastSeen = Timestamp.FromDateTime(DateTime.SpecifyKind(dto.LastSeen.Value, DateTimeKind.Utc));

        if (dto.Tags != null)
            protoDto.Tags.AddRange(dto.Tags);

        if (dto.AdditionalData != null)
            foreach (var kv in dto.AdditionalData)
                protoDto.AdditionalData[kv.Key] = kv.Value;

        return protoDto;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _channel?.Dispose();
        _disposed = true;
    }
}