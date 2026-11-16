using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ConfigurationController : Controller
{
    private readonly ApplicationDbContext _db;

    public ConfigurationController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.Organizations = await _db.HealthcareOrganizations
            .AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        ViewBag.Branches = await _db.Branches
            .AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        ViewBag.Departments = await _db.Departments
            .AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        ViewBag.Services = await _db.HealthcareServices
            .AsNoTracking().OrderBy(x => x.Name).ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrganization(
        string name, ApprovalMode approvalMode)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Trim().Length > 200 ||
            !Enum.IsDefined(approvalMode))
            return BadRequest("Invalid organization information.");

        _db.HealthcareOrganizations.Add(
            new HealthcareOrganization(name, approvalMode));

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Organization created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBranch(
        int organizationId, string name, string address)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Trim().Length > 200 ||
            string.IsNullOrWhiteSpace(address) ||
            address.Trim().Length > 500)
            return BadRequest("Invalid branch information.");

        var exists = await _db.HealthcareOrganizations
            .AnyAsync(x => x.Id == organizationId && x.IsActive);

        if (!exists)
            return BadRequest("Organization not found.");

        _db.Branches.Add(
            new Branch(organizationId, name, address));

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Branch created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDepartment(
        int branchId, string name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Trim().Length > 200)
            return BadRequest("Invalid department name.");

        var exists = await _db.Branches
            .AnyAsync(x => x.Id == branchId && x.IsActive);

        if (!exists)
            return BadRequest("Branch not found.");

        var duplicate = await _db.Departments.AnyAsync(
            x => x.BranchId == branchId && x.Name == name.Trim());

        if (duplicate)
        {
            TempData["ErrorMessage"] =
                "This department already exists in the selected branch.";
            return RedirectToAction(nameof(Index));
        }

        _db.Departments.Add(new Department(branchId, name));
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Department created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddService(
        string name, int durationMinutes, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Trim().Length > 200 ||
            durationMinutes <= 0 ||
            price < 0)
            return BadRequest("Invalid service information.");

        _db.HealthcareServices.Add(
            new HealthcareService(name, durationMinutes, price));

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Healthcare service created.";
        return RedirectToAction(nameof(Index));
    }
}
