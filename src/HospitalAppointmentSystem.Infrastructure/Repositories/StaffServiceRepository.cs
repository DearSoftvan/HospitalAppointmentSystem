using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class StaffServiceRepository : IStaffServiceRepository
{
    private readonly ApplicationDbContext _context;

    public StaffServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanProvideServiceAsync(
        int staffId,
        int healthcareServiceId)
    {
        return await _context.StaffServices
            .AsNoTracking()
            .AnyAsync(x =>
                x.StaffId == staffId &&
                x.HealthcareServiceId == healthcareServiceId);
    }
}
