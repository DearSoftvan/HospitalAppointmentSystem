using HospitalAppointmentSystem.Domain.Entities;

namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IHealthcareServiceRepository
{
    Task<HealthcareService?> GetByIdAsync(int id);
}
