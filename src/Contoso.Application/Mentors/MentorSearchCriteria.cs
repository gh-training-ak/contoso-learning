using Contoso.Domain.Enums;
using Contoso.Domain.ValueObjects;

namespace Contoso.Application.Abstractions;

public sealed record MentorSearchCriteria
{
    public Subject? Subject { get; init; }
    public MeetingType? MeetingType { get; init; }
    public decimal? MaxHourlyRate { get; init; }
    public decimal? MinimumRating { get; init; }
    public Address? Near { get; init; }
    public double? WithinKm { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public string CacheKey =>
        string.Join(':', "mentors", Subject?.ToString() ?? "any", MeetingType?.ToString() ?? "any",
                    MaxHourlyRate?.ToString("0.##") ?? "any", MinimumRating?.ToString("0.#") ?? "any",
                    WithinKm?.ToString("0") ?? "any", Page, PageSize);
}
