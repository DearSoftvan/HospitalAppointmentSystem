namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IBranchWorkingHoursRepository
{
    Task<bool> IsWithinWorkingHoursAsync(
        int branchId,
        DateTime start,
        DateTime end);
}
