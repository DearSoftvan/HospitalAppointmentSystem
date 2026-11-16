using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Domain.ValueObjects;

namespace HospitalAppointmentSystem.Domain.Entities;

public class Appointment
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public int StaffId { get; private set; }
    public int BranchId { get; private set; }
    public int HealthcareServiceId { get; private set; }

    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    public decimal Price { get; private set; }

    public AppointmentStatus Status { get; private set; }

    private Appointment() { }

    public Appointment(
        int patientId,
        int staffId,
        int branchId,
        int healthcareServiceId,
        DateTime start,
        DateTime end,
        decimal price,
        AppointmentStatus initialStatus)
    {
        if (patientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(patientId));

        if (staffId <= 0)
            throw new ArgumentOutOfRangeException(nameof(staffId));

        if (branchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(branchId));

        if (healthcareServiceId <= 0)
            throw new ArgumentOutOfRangeException(nameof(healthcareServiceId));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price));

        if (initialStatus != AppointmentStatus.Pending &&
            initialStatus != AppointmentStatus.Approved)
            throw new ArgumentException(
                "Initial status must be Pending or Approved.",
                nameof(initialStatus));

        var range = new TimeRange(start, end);

        PatientId = patientId;
        StaffId = staffId;
        BranchId = branchId;
        HealthcareServiceId = healthcareServiceId;
        Start = range.Start;
        End = range.End;
        Price = price;
        Status = initialStatus;
    }

    public TimeRange GetTimeRange()
    {
        return new TimeRange(Start, End);
    }

    public void Approve()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException(
                "Only pending appointments can be approved.");

        Status = AppointmentStatus.Approved;
    }

    public void Reject()
    {
        if (Status != AppointmentStatus.Pending)
            throw new InvalidOperationException(
                "Only pending appointments can be rejected.");

        Status = AppointmentStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status != AppointmentStatus.Pending &&
            Status != AppointmentStatus.Approved)
            throw new InvalidOperationException(
                "This appointment cannot be cancelled.");

        Status = AppointmentStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Approved)
            throw new InvalidOperationException(
                "Only approved appointments can be completed.");

        Status = AppointmentStatus.Completed;
    }
}
