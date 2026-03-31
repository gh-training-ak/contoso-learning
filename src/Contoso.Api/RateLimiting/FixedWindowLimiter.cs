namespace Contoso.Api.RateLimiting;

public sealed record RateLimitOptions(int PermitLimit, TimeSpan Window);

public sealed class FixedWindowLimiter(RateLimitOptions options, TimeProvider clock)
{
    private readonly Dictionary<string, Bucket> _buckets = new();

    public bool TryAcquire(string clientId)
    {
        var now = clock.GetUtcNow();

        if (!_buckets.TryGetValue(clientId, out var bucket) || now - bucket.WindowStart >= options.Window)
        {
            bucket = new Bucket(1, now);
        }
        else
        {
            bucket = bucket with { Count = bucket.Count + 1 };
        }

        _buckets[clientId] = bucket;

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
        return remaining > TimeSpan.Zero ? remaining + TimeSpan.FromSeconds(1) : TimeSpan.Zero;
    }

    private sealed record Bucket(int Count, DateTimeOffset WindowStart);
}
