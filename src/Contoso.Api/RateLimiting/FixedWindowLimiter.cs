using System.Collections.Concurrent;

namespace Contoso.Api.RateLimiting;

public sealed record RateLimitOptions(int PermitLimit, TimeSpan Window);

public sealed class FixedWindowLimiter(RateLimitOptions options, TimeProvider clock)
{
    private readonly ConcurrentDictionary<string, Bucket> _buckets = new();

    public bool TryAcquire(string clientId)
    {
        var now = clock.GetUtcNow();

        var bucket = _buckets.AddOrUpdate(
            clientId,
            _ => new Bucket(1, now),
            (_, existing) => now - existing.WindowStart >= options.Window
                ? new Bucket(1, now)
                : existing with { Count = existing.Count + 1 });

        return bucket.Count <= options.PermitLimit;
    }

    public TimeSpan RetryAfter(string clientId)
    {
        if (!_buckets.TryGetValue(clientId, out var bucket))
        {
            return TimeSpan.Zero;
        }

        var elapsed = clock.GetUtcNow() - bucket.WindowStart;
        var remaining = options.Window - elapsed;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    private sealed record Bucket(int Count, DateTimeOffset WindowStart);
}
