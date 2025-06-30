using Contoso.Domain.Common;
using Contoso.Domain.Entities;
using Contoso.Domain.Enums;

namespace Contoso.Tests.Domain;

public sealed class MatchRequestTests
{
    private static readonly DateTimeOffset Now = new(2026, 3, 2, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void NewRequestIsPending()
    {
        Assert.Equal(MatchStatus.Pending, new MatchRequest().Status);
    }

    [Fact]
    public void AcceptSetsStatusAndTimestamp()
    {
        var request = new MatchRequest();

        request.Accept(Now);

        Assert.Equal(MatchStatus.Accepted, request.Status);
        Assert.Equal(Now, request.AnsweredAt);
    }

    [Fact]
    public void RejectCapturesTheReason()
    {
        var request = new MatchRequest();

        request.Reject(Now, "Fully booked this term.");

        Assert.Equal(MatchStatus.Rejected, request.Status);
        Assert.Equal("Fully booked this term.", request.DeclineReason);
    }

    [Fact]
    public void AnsweringTwiceThrows()
    {
        var request = new MatchRequest();
        request.Accept(Now);

        var ex = Assert.Throws<DomainException>(() => request.Reject(Now));
        Assert.Contains("already answered", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WithdrawIsOnlyValidWhilePending()
    {
        var request = new MatchRequest();
        request.Withdraw(Now);

        Assert.Equal(MatchStatus.Withdrawn, request.Status);
    }
}
