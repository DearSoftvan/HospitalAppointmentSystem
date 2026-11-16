using HospitalAppointmentSystem.Domain.Entities;

namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id);
}
