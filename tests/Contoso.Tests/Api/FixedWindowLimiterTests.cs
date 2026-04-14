using Contoso.Api.RateLimiting;

namespace Contoso.Tests.Api;

public sealed class FixedWindowLimiterTests
{
    private sealed class MutableClock(DateTimeOffset start) : TimeProvider
    {
        private DateTimeOffset _now = start;
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan by) => _now = _now.Add(by);
    }

    [Fact]
    public void RequestsUnderTheLimitAreAllowed()
    {
        var limiter = new FixedWindowLimiter(new RateLimitOptions(3, TimeSpan.FromMinutes(1)),
                                             new MutableClock(DateTimeOffset.UnixEpoch));

        Assert.True(limiter.TryAcquire("client"));
        Assert.True(limiter.TryAcquire("client"));
        Assert.True(limiter.TryAcquire("client"));
    }

    [Fact]
    public void RequestOverTheLimitIsRefused()
    {
        var limiter = new FixedWindowLimiter(new RateLimitOptions(2, TimeSpan.FromMinutes(1)),
                                             new MutableClock(DateTimeOffset.UnixEpoch));

        limiter.TryAcquire("client");
        limiter.TryAcquire("client");

        Assert.False(limiter.TryAcquire("client"));
    }

    [Fact]
    public void WindowResetsExactlyOnTheBoundary()
    {
        var clock = new MutableClock(DateTimeOffset.UnixEpoch);
        var limiter = new FixedWindowLimiter(new RateLimitOptions(1, TimeSpan.FromMinutes(1)), clock);

        Assert.True(limiter.TryAcquire("client"));
        Assert.False(limiter.TryAcquire("client"));

        clock.Advance(TimeSpan.FromMinutes(1));

        Assert.True(limiter.TryAcquire("client"));
    }

    [Fact]
    public void ClientsAreTrackedSeparately()
    {
        var limiter = new FixedWindowLimiter(new RateLimitOptions(1, TimeSpan.FromMinutes(1)),
                                             new MutableClock(DateTimeOffset.UnixEpoch));

        Assert.True(limiter.TryAcquire("first"));
        Assert.True(limiter.TryAcquire("second"));
    }
}
