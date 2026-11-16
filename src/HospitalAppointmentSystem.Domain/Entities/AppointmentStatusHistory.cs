using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

public class AppointmentStatusHistory
{
    public int Id { get; private set; }
    public int AppointmentId { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public string? Note { get; private set; }

    private AppointmentStatusHistory() { }

    public AppointmentStatusHistory(
        int appointmentId,
        AppointmentStatus status,
        DateTime changedAt,
        string? note = null)
    {
        if (appointmentId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(appointmentId));

        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));

        AppointmentId = appointmentId;
        Status = status;
        ChangedAt = changedAt;
        Note = note?.Trim();
    }
}
