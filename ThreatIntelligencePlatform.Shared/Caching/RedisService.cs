using System.Text.Json;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;
using StackExchange.Redis;

namespace ThreatIntelligencePlatform.Shared.Caching;

public class RedisService : IRedisService
{
    private const string GlobalWhitelistLockKey = "lock:global:whitelist";
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
        _database = _redis.GetDatabase();
    }

    private async Task WithLockAsync(string key, Func<Task> action)
    {
        var lockKey = $"lock:{key}";
        var distributedLock = new RedisDistributedLock(lockKey, _database);

        await using (await distributedLock.AcquireAsync(TimeSpan.FromSeconds(30)))
        {
            await action();
        }
    }

    private async Task WithGlobalWhitelistLockAsync(Func<Task> action, TimeSpan? timeout = null)
    {
        var distributedLock = new RedisDistributedLock(GlobalWhitelistLockKey, _database);
        var lockTimeout = timeout ?? TimeSpan.FromMinutes(5);

        await using (await distributedLock.AcquireAsync(lockTimeout))
        {
            await action();
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
        {
            options.SetAbsoluteExpiration(expiry.Value);
        }

        if (IsWhitelistKey(key))
        {
            await WithGlobalWhitelistLockAsync(async () =>
            {
                await _cache.SetStringAsync(key, json, options);
            }, TimeSpan.FromMinutes(5));
        }
        else
        {
            await WithLockAsync(key, async () =>
            {
                await _cache.SetStringAsync(key, json, options);
            });
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (IsWhitelistKey(key))
        {
            T? result = default;
            await WithGlobalWhitelistLockAsync(async () =>
            {
                var json = await _cache.GetStringAsync(key);
                result = json is not null ? JsonSerializer.Deserialize<T>(json) : default;
            }, TimeSpan.FromSeconds(5));
            return result;
        }
        
        var json = await _cache.GetStringAsync(key);
        return json is not null ? JsonSerializer.Deserialize<T>(json) : default;
    }

    private bool IsWhitelistKey(string key)
    {
        return key.Contains(":") && 
               key.Split(':', 2)[0].Contains("whitelist", StringComparison.OrdinalIgnoreCase);
    }

    public IEnumerable<string> GetKeysByPattern(string pattern)
    {
        try
        {
            var endpoint = _redis.GetEndPoints().FirstOrDefault();
            if (endpoint is null) return [];

            var server = _redis.GetServer(endpoint);
            return server.Keys(pattern: pattern).Select(k => k.ToString()).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while retrieving keys by pattern {Pattern} from Redis", pattern);
            return [];
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
            if (pattern.StartsWith("whitelist", StringComparison.OrdinalIgnoreCase))
            {
                await WithGlobalWhitelistLockAsync(async () =>
                {
                    await DeleteKeysByPatternAsync(pattern);
                });
            }
            else
            {
                await DeleteKeysByPatternAsync(pattern);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while removing keys by pattern {Pattern}", pattern);
        }
    }

    private async Task DeleteKeysByPatternAsync(string pattern)
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
    
    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<bool> IsInWhitelistAsync(string source, string value)
    {
        var key = $"whitelist:{source}:{value}";
        bool exists = false;
        
        await WithGlobalWhitelistLockAsync(async () =>
        {
            exists = await _database.KeyExistsAsync(key);
        }, TimeSpan.FromSeconds(5));
        
        return exists;
    }

    public async Task AddToWhitelistAsync(string source, string value, TimeSpan expiry)
    {
        var key = $"whitelist:{source}:{value}";
        
        await WithGlobalWhitelistLockAsync(async () =>
        {
            await _database.StringSetAsync(key, value, expiry);
        }, TimeSpan.FromSeconds(30));
    }

    public async Task AddToWhitelistBatchAsync(string source, IEnumerable<string> values, TimeSpan expiry)
    {
        var keyValuePairs = values.Select(value => new KeyValuePair<RedisKey, RedisValue>(
            $"whitelist:{source}:{value}", 
            value
        )).ToList();

        await WithGlobalWhitelistLockAsync(async () =>
        {
            var batch = _database.CreateBatch();
            var tasks = keyValuePairs.Select(kv => 
                batch.StringSetAsync(kv.Key, kv.Value, expiry)).ToList();
            
            batch.Execute();
            await Task.WhenAll(tasks);
        }, TimeSpan.FromMinutes(5));
    }
}