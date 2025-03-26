using Contoso.Domain.Entities;

namespace Contoso.Application.Abstractions;

public interface IMentorRepository
{
    Task<Mentor?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Mentor>> SearchAsync(MentorSearchCriteria criteria, CancellationToken ct = default);
    Task<int> CountAsync(MentorSearchCriteria criteria, CancellationToken ct = default);
    Task AddAsync(Mentor mentor, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
