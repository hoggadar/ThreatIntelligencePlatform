using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace ThreatIntelligencePlatform.Worker.WhitelistCollector.Caching;

public class RedisService : IRedisService
{
    private readonly IDistributedCache _cache;

    public RedisService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions();

        if (expiry.HasValue)
            options.SetAbsoluteExpiration(expiry.Value);

        await _cache.SetStringAsync(key, json, options);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json is not null ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var value = await _cache.GetAsync(key);
        return value is not null;
    }
}