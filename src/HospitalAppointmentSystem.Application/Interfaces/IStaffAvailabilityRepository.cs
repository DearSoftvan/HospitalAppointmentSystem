namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IStaffAvailabilityRepository
{
    Task<bool> IsAvailableAsync(
        int staffId,
        DateTime start,
        DateTime end);
}
