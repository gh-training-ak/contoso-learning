using Contoso.Domain.Common;
using Contoso.Domain.Enums;

namespace Contoso.Domain.Entities;

public sealed class MatchRequest : Entity
{
    public Guid StudentId { get; init; }
    public Guid MentorId { get; init; }
    public MatchStatus Status { get; private set; } = MatchStatus.Pending;
    public DateTimeOffset? AnsweredAt { get; private set; }
    public string? DeclineReason { get; private set; }

    public void Accept(DateTimeOffset when)
    {
        EnsurePending();
        Status = MatchStatus.Accepted;
        AnsweredAt = when;
    }

    public void Reject(DateTimeOffset when, string? reason = null)
    {
        EnsurePending();
        Status = MatchStatus.Rejected;
        AnsweredAt = when;
        DeclineReason = reason;
    }

    public void Withdraw(DateTimeOffset when)
    {
        EnsurePending();
        Status = MatchStatus.Withdrawn;
        AnsweredAt = when;
    }

    private void EnsurePending()
    {
        if (Status is not MatchStatus.Pending)
        {
            throw new DomainException($"Request {Id} was already answered with {Status}.");
        }
    }
}
