using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class StaffAvailabilityRepository : IStaffAvailabilityRepository
{
    private readonly ApplicationDbContext _context;

    public StaffAvailabilityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsAvailableAsync(
        int staffId,
        DateTime start,
        DateTime end)
    {
        if (end <= start)
            return false;

        return await _context.StaffAvailabilities
            .AsNoTracking()
            .AnyAsync(x =>
                x.StaffId == staffId &&
                x.Start <= start &&
                x.End >= end);
    }
}
