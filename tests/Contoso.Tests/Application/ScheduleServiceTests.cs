using Contoso.Application.Scheduling;
using Contoso.Domain.Common;
using Contoso.Domain.Entities;

namespace Contoso.Tests.Application;

public sealed class ScheduleServiceTests
{
    private static ScheduleSlot Slot(DayOfWeek day, int from, int to, bool recurring = true) => new()
    {
        Day = day,
        StartsAt = new TimeOnly(from, 0),
        EndsAt = new TimeOnly(to, 0),
        IsRecurring = recurring
    };

    [Fact]
    public void ClashingSlotIsRejected()
    {
        var existing = new[] { Slot(DayOfWeek.Monday, 9, 11) };

        Assert.Throws<DomainException>(
            () => ScheduleService.EnsureNoOverlap(existing, Slot(DayOfWeek.Monday, 10, 12)));
    }

    [Fact]
    public void NonClashingSlotIsAccepted()
    {
        var existing = new[] { Slot(DayOfWeek.Monday, 9, 11) };

        ScheduleService.EnsureNoOverlap(existing, Slot(DayOfWeek.Monday, 11, 13));
    }

    [Fact]
    public void NextOccurrencesReturnsTheRequestedCount()
    {
        var slot = Slot(DayOfWeek.Wednesday, 16, 17);

        var dates = ScheduleService.NextOccurrences(slot, new DateOnly(2026, 3, 2), 4);

        Assert.Equal(4, dates.Count);
        Assert.All(dates, d => Assert.Equal(DayOfWeek.Wednesday, d.DayOfWeek));
    }

    [Fact]
    public void NonRecurringSlotHasNoOccurrences()
    {
        var slot = Slot(DayOfWeek.Wednesday, 16, 17, recurring: false);

        Assert.Empty(ScheduleService.NextOccurrences(slot, new DateOnly(2026, 3, 2), 4));
    }
}
