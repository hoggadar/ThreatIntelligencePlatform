using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using StackExchange.Redis;

namespace ThreatIntelligencePlatform.Worker.WhitelistCollector.Caching;

public class RedisService : IRedisService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
        _database = _redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions();

        if (expiry.HasValue)
        {
            options.SetAbsoluteExpiration(expiry.Value);
        }

        await _cache.SetStringAsync(key, json, options);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json is not null ? JsonSerializer.Deserialize<T>(json) : default;
    }
    
    public IEnumerable<string> GetKeysByPattern(string pattern)
    {
        try
        {
            var endpoint = _redis.GetEndPoints().FirstOrDefault();
            if (endpoint is null) return Enumerable.Empty<string>();

            var server = _redis.GetServer(endpoint);
            return server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while retrieving keys by pattern {Pattern} from Redis", pattern);
            return Enumerable.Empty<string>();
        }
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
    
    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            var keys = GetKeysByPattern(pattern);
            if (!keys.Any())
            {
                Log.Information("No keys found matching pattern {Pattern}, nothing to remove.", pattern);
                return;
            }

            const int batchSize = 500;
            var deleteTasks = keys.Chunk(batchSize)
                .Select(batch => _database.KeyDeleteAsync(batch.Select(k => (RedisKey)k).ToArray()));

            await Task.WhenAll(deleteTasks);
        
            Log.Information("Removed {Count} keys matching pattern {Pattern}", keys.Count(), pattern);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while removing keys by pattern {Pattern}", pattern);
        }
    }
    
    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
}