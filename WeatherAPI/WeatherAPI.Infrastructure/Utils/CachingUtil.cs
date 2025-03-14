using System.Collections.Concurrent;
using WeatherAPI.Application.Interfaces.Utils;

namespace WeatherAPI.Infrastructure.Utils
{
    public class CachingUtil : ICachingUtil
    {
        private static readonly ConcurrentDictionary<string, (object Data, DateTime Expiration)> _cache = new();
        private static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromMinutes(10);

        public bool TryGet(string key, out object? data)
        {
            data = null;

            key = string.IsNullOrWhiteSpace(key) ? "*" : key;
            if (!_cache.TryGetValue(key, out var cacheEntry)) return false;
            if (cacheEntry.Expiration <= DateTime.UtcNow)
            {
                Remove(key);
                return false;
            }

            data = cacheEntry.Data;
            return true;
        }

        public void Add(string key, object data, TimeSpan? duration = null)
        {
            key = string.IsNullOrWhiteSpace(key) ? "*" : key;
            var expiration = DateTime.UtcNow.Add(duration ?? DefaultCacheDuration);
            _cache[key] = (data, expiration);
        }

        public void Remove(string key)
        {
            key = string.IsNullOrWhiteSpace(key) ? "*" : key;
            _cache.TryRemove(key, out _);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
