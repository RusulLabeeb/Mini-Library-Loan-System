using BookStore.Application.Interfaces;
using System.Collections.Concurrent;

namespace BookStore.Infrastructure.Services;

public class SimpleDictionaryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, (object Value, DateTime Expiration)> _cache = new();

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiration > DateTime.UtcNow)
            {
                return (T)entry.Value;
            }

            _cache.TryRemove(key, out _);
        }

        return default;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        if (value != null)
        {
            _cache[key] = (value, DateTime.UtcNow.Add(expiration));
        }
    }

    public void Remove(string key)
    {
        _cache.TryRemove(key, out _);
    }
}
