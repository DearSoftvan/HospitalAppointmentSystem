using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthcareOrganization?> GetByIdAsync(int id)
    {
        return await _context.HealthcareOrganizations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
