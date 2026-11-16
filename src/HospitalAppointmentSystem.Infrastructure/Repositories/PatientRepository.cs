using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
