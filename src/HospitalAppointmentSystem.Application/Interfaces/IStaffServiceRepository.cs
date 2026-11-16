namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IStaffServiceRepository
{
    Task<bool> CanProvideServiceAsync(
        int staffId,
        int healthcareServiceId);
}
