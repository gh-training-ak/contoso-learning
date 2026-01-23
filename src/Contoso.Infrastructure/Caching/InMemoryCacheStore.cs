using System.Collections.Concurrent;
using Contoso.Application.Abstractions;

namespace Contoso.Infrastructure.Caching;

/// Used in development and tests. Production swaps this for the Redis implementation.
public sealed class InMemoryCacheStore(TimeProvider clock) : ICacheStore
{
    private readonly ConcurrentDictionary<string, (object Value, DateTimeOffset ExpiresAt)> _entries = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt > clock.GetUtcNow())
            {
                return Task.FromResult(entry.Value as T);
            }

            _entries.TryRemove(key, out _);
        }

        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class
    {
        _entries[key] = (value, clock.GetUtcNow().Add(ttl));
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _entries.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public int Count => _entries.Count;
}
