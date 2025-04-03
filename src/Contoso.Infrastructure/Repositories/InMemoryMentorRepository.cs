using Contoso.Application.Abstractions;
using Contoso.Domain.Entities;

namespace Contoso.Infrastructure.Repositories;

public sealed class InMemoryMentorRepository : IMentorRepository
{
    private readonly List<Mentor> _mentors = [];

    public void Seed(params Mentor[] mentors) => _mentors.AddRange(mentors);

    public Task<Mentor?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_mentors.FirstOrDefault(m => m.Id == id));

    public Task<IReadOnlyList<Mentor>> SearchAsync(MentorSearchCriteria criteria, CancellationToken ct = default)
    {
        IReadOnlyList<Mentor> page = Filter(criteria)
            .OrderBy(m => m.DisplayName, StringComparer.OrdinalIgnoreCase)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToList();

        return Task.FromResult(page);
    }

    public Task<int> CountAsync(MentorSearchCriteria criteria, CancellationToken ct = default)
        => Task.FromResult(Filter(criteria).Count());

    public Task AddAsync(Mentor mentor, CancellationToken ct = default)
    {
        _mentors.Add(mentor);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(0);

    private IEnumerable<Mentor> Filter(MentorSearchCriteria criteria)
    {
        var query = _mentors.Where(m => m.IsActive);

        if (criteria.MaxHourlyRate is { } rate)
        {
            query = query.Where(m => m.HourlyRate.Amount <= rate);
        }

        if (criteria.MeetingType is Domain.Enums.MeetingType.Online)
        {
            query = query.Where(m => m.AcceptsOnline);
        }

        if (criteria.MeetingType is Domain.Enums.MeetingType.InPerson)
        {
            query = query.Where(m => m.AcceptsInPerson);
        }

        return query;
    }
}
