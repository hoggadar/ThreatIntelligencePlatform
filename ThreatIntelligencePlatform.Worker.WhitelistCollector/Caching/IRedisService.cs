namespace ThreatIntelligencePlatform.Worker.WhitelistCollector.Caching;

public interface IRedisService
{
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key);
    IEnumerable<string> GetKeysByPattern(string pattern);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
}