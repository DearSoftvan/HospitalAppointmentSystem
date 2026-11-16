using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Repositories;

public class StaffRepository : IStaffRepository
{
    private readonly ApplicationDbContext _context;

    public StaffRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        return await _context.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
