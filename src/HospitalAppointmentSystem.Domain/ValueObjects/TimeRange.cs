namespace HospitalAppointmentSystem.Domain.ValueObjects;

public sealed record TimeRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public TimeSpan Duration => End - Start;

    public TimeRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException(
                "End time must be later than start time.");

        Start = start;
        End = end;
    }

    public bool Overlaps(TimeRange other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Start < other.End && End > other.Start;
    }

    public bool Contains(TimeRange other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Start <= other.Start && End >= other.End;
    }
}
