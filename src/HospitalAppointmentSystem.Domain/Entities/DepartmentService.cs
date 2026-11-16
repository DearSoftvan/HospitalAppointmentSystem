namespace HospitalAppointmentSystem.Domain.Entities;

public class DepartmentService
{
    public int DepartmentId { get; private set; }
    public int HealthcareServiceId { get; private set; }

    private DepartmentService() { }

    public DepartmentService(
        int departmentId,
        int healthcareServiceId)
    {
        if (departmentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(departmentId));

        if (healthcareServiceId <= 0)
            throw new ArgumentOutOfRangeException(nameof(healthcareServiceId));

        DepartmentId = departmentId;
        HealthcareServiceId = healthcareServiceId;
    }
}
