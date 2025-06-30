using Contoso.Application.Matching;
using Contoso.Domain.Common;
using Contoso.Domain.Entities;
using Contoso.Domain.Enums;
using Contoso.Infrastructure.Auditing;
using Contoso.Infrastructure.Repositories;

namespace Contoso.Tests.Application;

public sealed class MatchRequestServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 3, 2, 9, 0, 0, TimeSpan.Zero);

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private static (MatchRequestService Service, InMemoryMentorRepository Mentors, InMemoryAuditSink Audit) Build()
    {
        var mentors = new InMemoryMentorRepository();
        var requests = new InMemoryMatchRequestRepository();
        var audit = new InMemoryAuditSink();
        return (new MatchRequestService(mentors, requests, audit, new FixedClock(Now)), mentors, audit);
    }

    private static Mentor SeedMentor(InMemoryMentorRepository repo)
    {
        var mentor = new Mentor { DisplayName = "Available Mentor", Email = "available@contoso.example" };
        repo.Seed(mentor);
        return mentor;
    }

    [Fact]
    public async Task RequestingAnUnknownMentorThrows()
    {
        var (service, _, _) = Build();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.RequestAsync(Guid.NewGuid(), Guid.NewGuid(), "student@contoso.example"));
    }

    [Fact]
    public async Task SuccessfulRequestWritesAnAuditEntry()
    {
        var (service, mentors, audit) = Build();
        var mentor = SeedMentor(mentors);

        await service.RequestAsync(Guid.NewGuid(), mentor.Id, "student@contoso.example");

        Assert.Single(audit.Entries);
        Assert.Equal("match-request.created", audit.Entries.First().Action);
    }

    [Fact]
    public async Task DuplicateRequestIsRejected()
    {
        var (service, mentors, _) = Build();
        var mentor = SeedMentor(mentors);
        var studentId = Guid.NewGuid();

        await service.RequestAsync(studentId, mentor.Id, "student@contoso.example");

        await Assert.ThrowsAsync<DomainException>(
            () => service.RequestAsync(studentId, mentor.Id, "student@contoso.example"));
    }

    [Fact]
    public async Task RequestingAnInactiveMentorThrows()
    {
        var (service, mentors, _) = Build();
        var mentor = SeedMentor(mentors);
        mentor.SoftDelete(Now);

        await Assert.ThrowsAsync<DomainException>(
            () => service.RequestAsync(Guid.NewGuid(), mentor.Id, "student@contoso.example"));
    }

    [Fact]
    public async Task AcceptingUpdatesStatusAndAudits()
    {
        var (service, mentors, audit) = Build();
        var mentor = SeedMentor(mentors);
        var created = await service.RequestAsync(Guid.NewGuid(), mentor.Id, "student@contoso.example");

        var answered = await service.AnswerAsync(created.Id, accept: true, "mentor@contoso.example");

        Assert.Equal(MatchStatus.Accepted, answered.Status);
        Assert.Equal(2, audit.Entries.Count);
    }

    [Fact]
    public void BookingInsideTwoDaysIsRefused()
    {
        var (service, _, _) = Build();

        Assert.False(service.CanBookSlot(Now.AddDays(1)));
        Assert.True(service.CanBookSlot(Now.AddDays(3)));
    }
}
