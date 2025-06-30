using Contoso.Domain.Entities;

namespace Contoso.Application.Abstractions;

public interface IMatchRequestRepository
{
    Task<MatchRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<MatchRequest>> ForMentorAsync(Guid mentorId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid studentId, Guid mentorId, CancellationToken ct = default);
    Task AddAsync(MatchRequest request, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
