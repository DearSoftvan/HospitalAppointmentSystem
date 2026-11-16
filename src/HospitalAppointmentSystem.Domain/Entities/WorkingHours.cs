namespace HospitalAppointmentSystem.Domain.Entities;

public class WorkingHours
{
    public int Id { get; private set; }
    public int BranchId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    private WorkingHours() { }

    public WorkingHours(
        int branchId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (branchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(branchId));

        if (!Enum.IsDefined(dayOfWeek))
            throw new ArgumentOutOfRangeException(nameof(dayOfWeek));

        if (endTime <= startTime)
            throw new ArgumentException(
                "End time must be later than start time.");

        BranchId = branchId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }

    public bool Contains(TimeOnly start, TimeOnly end)
    {
        return start >= StartTime
            && end <= EndTime
            && end > start;
    }
}
