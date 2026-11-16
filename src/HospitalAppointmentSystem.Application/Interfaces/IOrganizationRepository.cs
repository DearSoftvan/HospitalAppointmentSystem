using HospitalAppointmentSystem.Domain.Entities;

namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IOrganizationRepository
{
    Task<HealthcareOrganization?> GetByIdAsync(int id);
}
