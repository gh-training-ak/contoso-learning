using Contoso.Application.Abstractions;
using Contoso.Domain.Entities;

namespace Contoso.Infrastructure.Repositories;

public sealed class InMemoryMatchRequestRepository : IMatchRequestRepository
{
    private readonly List<MatchRequest> _requests = [];

    public Task<MatchRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_requests.FirstOrDefault(r => r.Id == id));

    public Task<IReadOnlyList<MatchRequest>> ForMentorAsync(Guid mentorId, CancellationToken ct = default)
    {
        IReadOnlyList<MatchRequest> list = _requests.Where(r => r.MentorId == mentorId).ToList();
        return Task.FromResult(list);
    }

    public Task<bool> ExistsAsync(Guid studentId, Guid mentorId, CancellationToken ct = default)
        => Task.FromResult(_requests.Any(r => r.StudentId == studentId && r.MentorId == mentorId));

    public Task AddAsync(MatchRequest request, CancellationToken ct = default)
    {
        _requests.Add(request);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(_requests.Count);
}
