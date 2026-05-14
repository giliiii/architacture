using BsdFinalProject.IServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BsdFinalProject.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly int _defaultTtlSeconds;

        public CacheService(IDistributedCache cache, IConfiguration config)
        {
            _cache = cache;
            _defaultTtlSeconds = int.TryParse(config["Redis:DefaultTTLSeconds"], out var ttl) ? ttl : 60;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (cachedData == null)
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromSeconds(_defaultTtlSeconds);

            var serializedData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedData, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task InvalidateByPatternAsync(string pattern)
        {
            // Note: IDistributedCache doesn't support pattern matching like Redis.
            // For simplicity, we'll assume keys are known and invalidate specific ones.
            // In a real scenario, you might need to use Redis or maintain a key registry.
            // For now, this is a placeholder.
            // You can implement a list of keys or use a different cache provider.
        }
    }
}