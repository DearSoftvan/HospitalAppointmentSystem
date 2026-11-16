using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class BranchWorkingHoursRepository
    : IBranchWorkingHoursRepository
{
    private readonly ApplicationDbContext _context;

    public BranchWorkingHoursRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsWithinWorkingHoursAsync(
        int branchId,
        DateTime start,
        DateTime end)
    {
        if (end <= start || start.Date != end.Date)
            return false;

        var workingHours = await _context.WorkingHours
            .AsNoTracking()
            .Where(x =>
                x.BranchId == branchId &&
                x.DayOfWeek == start.DayOfWeek)
            .ToListAsync();

        var startTime = TimeOnly.FromDateTime(start);
        var endTime = TimeOnly.FromDateTime(end);

        return workingHours.Any(x =>
            x.Contains(startTime, endTime));
    }
}
