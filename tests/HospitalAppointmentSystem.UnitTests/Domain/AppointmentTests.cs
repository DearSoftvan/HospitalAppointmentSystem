using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.UnitTests.Domain;

public class AppointmentTests
{
    private static Appointment CreateAppointment(
        AppointmentStatus status = AppointmentStatus.Pending)
    {
        return new Appointment(
            patientId: 1,
            staffId: 1,
            branchId: 1,
            healthcareServiceId: 1,
            start: new DateTime(2026, 11, 20, 10, 0, 0),
            end: new DateTime(2026, 11, 20, 10, 30, 0),
            price: 500m,
            initialStatus: status);
    }

    [Fact]
    public void Constructor_ShouldCreatePendingAppointment()
    {
        var appointment = CreateAppointment();

        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        Assert.Equal(500m, appointment.Price);
    }

    [Fact]
    public void Approve_ShouldChangePendingToApproved()
    {
        var appointment = CreateAppointment();

        appointment.Approve();

        Assert.Equal(AppointmentStatus.Approved, appointment.Status);
    }

    [Fact]
    public void Approve_ShouldRejectAlreadyApprovedAppointment()
    {
        var appointment = CreateAppointment(
            AppointmentStatus.Approved);

        Assert.Throws<InvalidOperationException>(() =>
            appointment.Approve());
    }

    [Fact]
    public void Reject_ShouldChangePendingToRejected()
    {
        var appointment = CreateAppointment();

        appointment.Reject();

        Assert.Equal(AppointmentStatus.Rejected, appointment.Status);
    }

    [Fact]
    public void Cancel_ShouldChangeApprovedToCancelled()
    {
        var appointment = CreateAppointment(
            AppointmentStatus.Approved);

        appointment.Cancel();

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }

    [Fact]
    public void Complete_ShouldRejectPendingAppointment()
    {
        var appointment = CreateAppointment();

        Assert.Throws<InvalidOperationException>(() =>
            appointment.Complete());
    }

    [Fact]
    public void Complete_ShouldChangeApprovedToCompleted()
    {
        var appointment = CreateAppointment(
            AppointmentStatus.Approved);

        appointment.Complete();

        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void Constructor_ShouldRejectNegativePrice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Appointment(
                1, 1, 1, 1,
                new DateTime(2026, 11, 20, 10, 0, 0),
                new DateTime(2026, 11, 20, 10, 30, 0),
                -100m,
                AppointmentStatus.Pending));
    }
}
