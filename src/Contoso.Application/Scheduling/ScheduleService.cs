using Contoso.Domain.Common;
using Contoso.Domain.Entities;

namespace Contoso.Application.Scheduling;

public sealed class ScheduleService
{
    public static void EnsureNoOverlap(IEnumerable<ScheduleSlot> existing, ScheduleSlot candidate)
    {
        ArgumentNullException.ThrowIfNull(existing);
        ArgumentNullException.ThrowIfNull(candidate);

        var clash = existing.FirstOrDefault(s => s.Id != candidate.Id && s.Overlaps(candidate));

        if (clash is not null)
        {
            var from = clash.StartsAt.ToString("HH:mm");
            var to = clash.EndsAt.ToString("HH:mm");
            throw new DomainException($"That slot clashes with {clash.Day} {from} to {to}.");
        }
    }

    public static IReadOnlyList<DateOnly> NextOccurrences(ScheduleSlot slot, DateOnly from, int count)
    {
        ArgumentNullException.ThrowIfNull(slot);

        if (!slot.IsRecurring)
        {
            return [];
        }

        var dates = new List<DateOnly>(count);
        var cursor = from;

        while (dates.Count < count)
        {
            if (cursor.DayOfWeek == slot.Day)
            {
                dates.Add(cursor);
            }

            cursor = cursor.AddDays(1);
        }

        return dates;
    }
}
