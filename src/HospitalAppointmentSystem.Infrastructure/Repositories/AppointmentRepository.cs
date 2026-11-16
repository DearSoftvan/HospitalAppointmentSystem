using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasConflictAsync(
        int staffId,
        int patientId,
        DateTime start,
        DateTime end)
    {
        if (end <= start)
            throw new ArgumentException(
                "Appointment end must be after start.");

        return await _context.Appointments
            .AsNoTracking()
            .AnyAsync(x =>
                (x.StaffId == staffId ||
                 x.PatientId == patientId) &&
                (x.Status == AppointmentStatus.Pending ||
                 x.Status == AppointmentStatus.Approved) &&
                x.Start < end &&
                x.End > start);
    }
}
