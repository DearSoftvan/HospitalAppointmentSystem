using HospitalAppointmentSystem.Domain.ValueObjects;

namespace HospitalAppointmentSystem.UnitTests.Domain;

public class TimeRangeTests
{
    [Fact]
    public void Constructor_ShouldRejectInvalidTimeRange()
    {
        var start = new DateTime(2026, 11, 20, 10, 0, 0);

        Assert.Throws<ArgumentException>(() =>
            new TimeRange(start, start));
    }

    [Fact]
    public void Overlaps_ShouldReturnTrue_WhenRangesOverlap()
    {
        var first = new TimeRange(
            new DateTime(2026, 11, 20, 9, 0, 0),
            new DateTime(2026, 11, 20, 10, 0, 0));

        var second = new TimeRange(
            new DateTime(2026, 11, 20, 9, 30, 0),
            new DateTime(2026, 11, 20, 10, 30, 0));

        Assert.True(first.Overlaps(second));
    }

    [Fact]
    public void Overlaps_ShouldReturnFalse_WhenRangesAreAdjacent()
    {
        var first = new TimeRange(
            new DateTime(2026, 11, 20, 9, 0, 0),
            new DateTime(2026, 11, 20, 10, 0, 0));

        var second = new TimeRange(
            new DateTime(2026, 11, 20, 10, 0, 0),
            new DateTime(2026, 11, 20, 11, 0, 0));

        Assert.False(first.Overlaps(second));
    }

    [Fact]
    public void Contains_ShouldReturnTrue_WhenRangeIsInside()
    {
        var availability = new TimeRange(
            new DateTime(2026, 11, 20, 9, 0, 0),
            new DateTime(2026, 11, 20, 17, 0, 0));

        var appointment = new TimeRange(
            new DateTime(2026, 11, 20, 10, 0, 0),
            new DateTime(2026, 11, 20, 10, 30, 0));

        Assert.True(availability.Contains(appointment));
    }
}
