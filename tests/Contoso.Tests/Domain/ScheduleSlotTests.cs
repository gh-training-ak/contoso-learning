using Contoso.Domain.Common;
using Contoso.Domain.Entities;

namespace Contoso.Tests.Domain;

public sealed class ScheduleSlotTests
{
    private static ScheduleSlot Slot(int from, int to, DayOfWeek day = DayOfWeek.Monday) => new()
    {
        Day = day,
        StartsAt = new TimeOnly(from, 0),
        EndsAt = new TimeOnly(to, 0)
    };

    [Fact]
    public void AdjacentSlotsDoNotOverlap()
    {
        Assert.False(Slot(9, 10).Overlaps(Slot(10, 11)));
    }

    [Fact]
    public void PartiallyOverlappingSlotsOverlap()
    {
        Assert.True(Slot(9, 11).Overlaps(Slot(10, 12)));
    }

    [Fact]
    public void IdenticalSlotsOverlap()
    {
        Assert.True(Slot(9, 11).Overlaps(Slot(9, 11)));
    }

    [Fact]
    public void SlotsOnDifferentDaysNeverOverlap()
    {
        Assert.False(Slot(9, 11).Overlaps(Slot(9, 11, DayOfWeek.Tuesday)));
    }

    [Fact]
    public void OvernightSlotThrowsOnDuration()
    {
        var overnight = new ScheduleSlot
        {
            Day = DayOfWeek.Friday,
            StartsAt = new TimeOnly(23, 0),
            EndsAt = new TimeOnly(0, 30)
        };

        Assert.Throws<DomainException>(() => overnight.Duration);
    }

    [Fact]
    public void DurationIsTheDifference()
    {
        Assert.Equal(TimeSpan.FromHours(2), Slot(9, 11).Duration);
    }
}
