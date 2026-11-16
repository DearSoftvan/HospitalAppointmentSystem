using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class StaffManagementController : Controller
{
    private readonly ApplicationDbContext _db;

    public StaffManagementController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Staff = await _db.StaffMembers
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ToListAsync();

        ViewBag.Branches = await _db.Branches
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        ViewBag.Services = await _db.HealthcareServices
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        ViewBag.Assignments = await _db.StaffServices
            .AsNoTracking()
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStaff(
        int branchId,
        string fullName,
        Profession profession,
        string? specialty)
    {
        if (string.IsNullOrWhiteSpace(fullName) ||
            fullName.Trim().Length > 200 ||
            !Enum.IsDefined(profession))
        {
            TempData["ErrorMessage"] = "Invalid staff information.";
            return RedirectToAction(nameof(Index));
        }

        var branchExists = await _db.Branches.AnyAsync(
            x => x.Id == branchId && x.IsActive);

        if (!branchExists)
        {
            TempData["ErrorMessage"] = "Selected branch is not active.";
            return RedirectToAction(nameof(Index));
        }

        Staff staff;

        switch (profession)
        {
            case Profession.Doctor:
                if (string.IsNullOrWhiteSpace(specialty))
                {
                    TempData["ErrorMessage"] =
                        "Doctor specialty is required.";
                    return RedirectToAction(nameof(Index));
                }

                staff = new Doctor(
                    branchId,
                    fullName,
                    specialty);
                break;

            case Profession.Nurse:
                staff = new Nurse(branchId, fullName);
                break;

            case Profession.LaboratoryTechnician:
                staff = new LaboratoryTechnician(branchId, fullName);
                break;

            default:
                return BadRequest("Unsupported profession.");
        }

        _db.StaffMembers.Add(staff);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"{profession} created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignService(
        int staffId,
        int healthcareServiceId)
    {
        var staffExists = await _db.StaffMembers.AnyAsync(
            x => x.Id == staffId && x.IsActive);

        var serviceExists = await _db.HealthcareServices.AnyAsync(
            x => x.Id == healthcareServiceId && x.IsActive);

        if (!staffExists || !serviceExists)
        {
            TempData["ErrorMessage"] =
                "Staff or healthcare service not found.";
            return RedirectToAction(nameof(Index));
        }

        var alreadyAssigned = await _db.StaffServices.AnyAsync(
            x => x.StaffId == staffId &&
                 x.HealthcareServiceId == healthcareServiceId);

        if (alreadyAssigned)
        {
            TempData["ErrorMessage"] =
                "This service is already assigned to the staff member.";
            return RedirectToAction(nameof(Index));
        }

        _db.StaffServices.Add(
            new StaffService(staffId, healthcareServiceId));

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Healthcare service assigned successfully.";

        return RedirectToAction(nameof(Index));
    }
}
