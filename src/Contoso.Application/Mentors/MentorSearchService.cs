using Contoso.Application.Abstractions;
using Contoso.Application.Common;
using Contoso.Domain.Entities;
using Contoso.Domain.ValueObjects;

namespace Contoso.Application.Mentors;

public sealed class MentorSearchService(IMentorRepository repository, ICacheStore cache)
{
    public const int MaxPageSize = 100;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    public async Task<PagedResult<MentorSearchResult>> SearchAsync(
        MentorSearchCriteria criteria, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        if (criteria.PageSize is < 1 or > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(criteria), $"PageSize must be between 1 and {MaxPageSize}.");
        }

        var cached = await cache.GetAsync<PagedResult<MentorSearchResult>>(criteria.CacheKey, ct);
        if (cached is not null)
        {
            return cached;
        }

        var mentors = await repository.SearchAsync(criteria, ct);
        var total = await repository.CountAsync(criteria, ct);

        var items = mentors
            .Where(m => m.IsActive)
            .Select(m => Project(m, criteria.Near))
            .Where(r => criteria.MinimumRating is null || r.Rating >= criteria.MinimumRating)
            .Where(r => criteria.WithinKm is null || r.DistanceKm is null || r.DistanceKm <= criteria.WithinKm)
            .OrderByDescending(r => r.Rating)
            .ThenBy(r => r.DistanceKm ?? double.MaxValue)
            .ToList();

        var result = new PagedResult<MentorSearchResult>(items, criteria.Page, criteria.PageSize, total);
        await cache.SetAsync(criteria.CacheKey, result, CacheTtl, ct);
        return result;
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
