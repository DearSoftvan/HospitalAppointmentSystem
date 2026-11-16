using HospitalAppointmentSystem.Domain.Entities;

namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IStaffRepository
{
    Task<Staff?> GetByIdAsync(int id);
}
