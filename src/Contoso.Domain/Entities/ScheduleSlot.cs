using Contoso.Domain.Common;

namespace Contoso.Domain.Entities;

public sealed class ScheduleSlot : Entity
{
    public Guid MentorId { get; init; }
    public DayOfWeek Day { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public bool IsRecurring { get; set; }

    public TimeSpan Duration => EndsAt >= StartsAt
        ? EndsAt - StartsAt
        : throw new DomainException("A slot cannot end before it starts. Overnight slots are not supported.");

    // Touching slots are allowed: a lesson ending at 10:00 and one starting at 10:00 do not clash.
    public bool Overlaps(ScheduleSlot other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return Day == other.Day && StartsAt < other.EndsAt && other.StartsAt < EndsAt;
    }
}
