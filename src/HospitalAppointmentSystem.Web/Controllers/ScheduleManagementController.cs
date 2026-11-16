using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ScheduleManagementController : Controller
{
    private readonly ApplicationDbContext _db;

    public ScheduleManagementController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Branches = await _db.Branches
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        ViewBag.Staff = await _db.StaffMembers
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.FullName)
            .ToListAsync();

        ViewBag.WorkingHours = await _db.WorkingHours
            .AsNoTracking()
            .OrderBy(x => x.BranchId)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        ViewBag.Availabilities = await _db.StaffAvailabilities
            .AsNoTracking()
            .Where(x => x.End >= DateTime.Today)
            .OrderBy(x => x.Start)
            .Take(200)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddWorkingHours(
        int branchId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (!Enum.IsDefined(dayOfWeek) || endTime <= startTime)
        {
            TempData["ErrorMessage"] = "Invalid working hours.";
            return RedirectToAction(nameof(Index));
        }

        var branchExists = await _db.Branches.AnyAsync(
            x => x.Id == branchId && x.IsActive);

        if (!branchExists)
        {
            TempData["ErrorMessage"] = "Branch not found.";
            return RedirectToAction(nameof(Index));
        }

        var overlapping = await _db.WorkingHours.AnyAsync(x =>
            x.BranchId == branchId &&
            x.DayOfWeek == dayOfWeek &&
            x.StartTime < endTime &&
            x.EndTime > startTime);

        if (overlapping)
        {
            TempData["ErrorMessage"] =
                "These working hours overlap an existing interval.";
            return RedirectToAction(nameof(Index));
        }

        _db.WorkingHours.Add(
            new WorkingHours(
                branchId, dayOfWeek, startTime, endTime));

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Working hours created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAvailability(
        int staffId,
        DateTime start,
        DateTime end)
    {
        if (start < DateTime.Now || end <= start ||
            start.Date != end.Date)
        {
            TempData["ErrorMessage"] =
                "Select a valid future interval within one day.";
            return RedirectToAction(nameof(Index));
        }

        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == staffId && x.IsActive);

        if (staff == null)
        {
            TempData["ErrorMessage"] = "Staff member not found.";
            return RedirectToAction(nameof(Index));
        }

        var overlapping = await _db.StaffAvailabilities.AnyAsync(x =>
            x.StaffId == staffId &&
            x.Start < end &&
            x.End > start);

        if (overlapping)
        {
            TempData["ErrorMessage"] =
                "This availability overlaps an existing interval.";
            return RedirectToAction(nameof(Index));
        }

        var branchHours = await _db.WorkingHours
            .AsNoTracking()
            .Where(x =>
                x.BranchId == staff.BranchId &&
                x.DayOfWeek == start.DayOfWeek)
            .ToListAsync();

        var startTime = TimeOnly.FromDateTime(start);
        var endTime = TimeOnly.FromDateTime(end);

        if (!branchHours.Any(x => x.Contains(startTime, endTime)))
        {
            TempData["ErrorMessage"] =
                "Availability must fit within branch working hours.";
            return RedirectToAction(nameof(Index));
        }

        _db.StaffAvailabilities.Add(
            new StaffAvailability(staffId, start, end));

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Staff availability created.";
        return RedirectToAction(nameof(Index));
    }
}
