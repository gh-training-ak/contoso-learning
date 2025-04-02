using Contoso.Application.Abstractions;
using Contoso.Domain.Entities;
using Contoso.Domain.ValueObjects;

namespace Contoso.Application.Mentors;

public sealed class MentorSearchService(IMentorRepository repository)
{
    public async Task<IReadOnlyList<MentorSearchResult>> SearchAsync(
        MentorSearchCriteria criteria, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        var mentors = await repository.SearchAsync(criteria, ct);

        return mentors
            .Where(m => m.IsActive)
            .Select(m => Project(m, criteria.Near))
            .Where(r => criteria.MinimumRating is null || r.Rating >= criteria.MinimumRating)
            .OrderByDescending(r => r.Rating)
            .ToList();
    }

    private static MentorSearchResult Project(Mentor mentor, Address? origin) => new(
        mentor.Id,
        mentor.DisplayName,
        mentor.HourlyRate.Amount,
        mentor.HourlyRate.Currency,
        mentor.AverageRating,
        mentor.Reviews.Count,
        mentor.Address is null || origin is null ? null : Math.Round(origin.DistanceKmTo(mentor.Address), 2));
}

public sealed record MentorSearchResult(
    Guid Id,
    string DisplayName,
    decimal HourlyRate,
    string Currency,
    decimal Rating,
    int ReviewCount,
    double? DistanceKm);
