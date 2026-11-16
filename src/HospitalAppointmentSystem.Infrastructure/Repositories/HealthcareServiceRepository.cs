using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class HealthcareServiceRepository : IHealthcareServiceRepository
{
    private readonly ApplicationDbContext _context;

    public HealthcareServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthcareService?> GetByIdAsync(int id)
    {
        return await _context.HealthcareServices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
