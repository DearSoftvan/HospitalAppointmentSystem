using HospitalAppointmentSystem.Domain.ValueObjects;

namespace HospitalAppointmentSystem.Domain.Entities;

public class StaffAvailability
{
    public int Id { get; private set; }
    public int StaffId { get; private set; }
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }

    private StaffAvailability() { }

    public StaffAvailability(
        int staffId,
        DateTime start,
        DateTime end)
    {
        if (staffId <= 0)
            throw new ArgumentOutOfRangeException(nameof(staffId));

        var range = new TimeRange(start, end);

        StaffId = staffId;
        Start = range.Start;
        End = range.End;
    }

    public bool Contains(TimeRange requestedRange)
    {
        var availability = new TimeRange(Start, End);
        return availability.Contains(requestedRange);
    }
}
