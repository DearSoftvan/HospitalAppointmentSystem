using HospitalAppointmentSystem.Domain.Entities;

namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(int id);
}
