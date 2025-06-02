using Contoso.Application.Abstractions;
using Contoso.Domain.Common;
using Contoso.Domain.Entities;

namespace Contoso.Application.Matching;

public sealed class MatchRequestService(
    IMentorRepository mentors,
    IMatchRequestRepository requests,
    IAuditSink audit,
    TimeProvider clock)
{
    public static readonly TimeSpan MinimumBookingNotice = TimeSpan.FromDays(2);

    public async Task<MatchRequest> RequestAsync(Guid studentId, Guid mentorId, string actor,
        CancellationToken ct = default)
    {
        var mentor = await mentors.GetByIdAsync(mentorId, ct)
            ?? throw new KeyNotFoundException($"Mentor {mentorId} was not found.");

        if (!mentor.IsActive)
        {
            throw new DomainException("That mentor is no longer accepting students.");
        }

        if (await requests.ExistsAsync(studentId, mentorId, ct))
        {
            throw new DomainException("You have already sent a request to this mentor.");
        }

        var request = new MatchRequest { StudentId = studentId, MentorId = mentorId };
        await requests.AddAsync(request, ct);
        await requests.SaveChangesAsync(ct);

        await audit.WriteAsync(
            AuditEntry.For(actor, "match-request.created", nameof(MatchRequest), request.Id, clock.GetUtcNow()),
            ct);

        return request;
    }

    public async Task<MatchRequest> AnswerAsync(Guid requestId, bool accept, string actor,
        string? reason = null, CancellationToken ct = default)
    {
        var request = await requests.GetByIdAsync(requestId, ct)
            ?? throw new KeyNotFoundException($"Request {requestId} was not found.");

        var now = clock.GetUtcNow();

        if (accept)
        {
            request.Accept(now);
        }
        else
        {
            request.Reject(now, reason);
        }

        await requests.SaveChangesAsync(ct);

        await audit.WriteAsync(
            AuditEntry.For(actor, accept ? "match-request.accepted" : "match-request.rejected",
                           nameof(MatchRequest), request.Id, now),
            ct);

        return request;
    }

    public bool CanBookSlot(DateTimeOffset slotStart) =>
        slotStart - clock.GetUtcNow() >= MinimumBookingNotice;
}
