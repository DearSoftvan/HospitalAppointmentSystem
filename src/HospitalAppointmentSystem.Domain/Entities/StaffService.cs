namespace HospitalAppointmentSystem.Domain.Entities;

public class StaffService
{
    public int StaffId { get; private set; }
    public int HealthcareServiceId { get; private set; }

    private StaffService() { }

    public StaffService(int staffId, int healthcareServiceId)
    {
        if (staffId <= 0)
            throw new ArgumentOutOfRangeException(nameof(staffId));

        if (healthcareServiceId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(healthcareServiceId));

        StaffId = staffId;
        HealthcareServiceId = healthcareServiceId;
    }
}
