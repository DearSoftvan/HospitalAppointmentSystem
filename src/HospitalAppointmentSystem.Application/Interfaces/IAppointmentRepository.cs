using HospitalAppointmentSystem.Domain.Entities;

namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);

    Task AddAsync(Appointment appointment);

    Task<bool> HasConflictAsync(
        int staffId,
        int patientId,
        DateTime start,
        DateTime end);
}
