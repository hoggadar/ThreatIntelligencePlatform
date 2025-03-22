namespace ThreatIntelligencePlatform.Shared.Caching;

public interface IRedisService
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    IEnumerable<string> GetKeysByPattern(string pattern);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
    Task<bool> IsInWhitelistAsync(string source, string value);
    Task AddToWhitelistAsync(string source, string value, TimeSpan expiry);
    Task AddToWhitelistBatchAsync(string source, IEnumerable<string> values, TimeSpan expiry);
}