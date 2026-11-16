using HospitalAppointmentSystem.Application.DTOs;
using HospitalAppointmentSystem.Application.Services;
using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly AppointmentService _appointmentService;

    public AppointmentsController(
        ApplicationDbContext db,
        AppointmentService appointmentService)
    {
        _db = db;
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateAppointmentViewModel
        {
            Start = DateTime.Today.AddDays(1).AddHours(10)
        };

        await PopulateSelectionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateAppointmentViewModel model)
    {
        var identityUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var patient = await _db.Patients
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.IdentityUserId == identityUserId && x.IsActive);

        if (patient == null)
            return Forbid();

        if (model.Start.HasValue &&
            model.Start.Value <= DateTime.Now)
        {
            ModelState.AddModelError(
                nameof(model.Start),
                "Please select a future appointment time.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateSelectionsAsync(model);
            return View(model);
        }

        var branchBelongsToOrganization = await _db.Branches
            .AnyAsync(x =>
                x.Id == model.BranchId &&
                x.OrganizationId == model.OrganizationId &&
                x.IsActive);

        if (!branchBelongsToOrganization)
        {
            ModelState.AddModelError(
                string.Empty,
                "The selected branch does not belong to the selected organization.");

            await PopulateSelectionsAsync(model);
            return View(model);
        }

        try
        {
            var request = new CreateAppointmentRequest(
                patient.Id,
                model.StaffId,
                model.BranchId,
                model.HealthcareServiceId,
                model.Start!.Value);

            await _appointmentService.CreateAsync(request);

            TempData["SuccessMessage"] =
                "Your appointment has been created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        await PopulateSelectionsAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var identityUserId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var patient = await _db.Patients
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.IdentityUserId == identityUserId);

        if (patient == null)
            return Forbid();

        var appointments = await (
            from appointment in _db.Appointments.AsNoTracking()
            join staff in _db.StaffMembers
                on appointment.StaffId equals staff.Id
            join branch in _db.Branches
                on appointment.BranchId equals branch.Id
            join service in _db.HealthcareServices
                on appointment.HealthcareServiceId equals service.Id
            where appointment.PatientId == patient.Id
            orderby appointment.Start descending
            select new AppointmentListItemViewModel
            {
                Id = appointment.Id,
                Start = appointment.Start,
                End = appointment.End,
                StaffName = staff.FullName,
                BranchName = branch.Name,
                ServiceName = service.Name,
                Status = appointment.Status.ToString()
            }).ToListAsync();

        return View(appointments);
    }

    private async Task PopulateSelectionsAsync(
        CreateAppointmentViewModel model)
    {
        model.Organizations = await _db.HealthcareOrganizations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(
                x.Name, x.Id.ToString()))
            .ToListAsync();

        model.Branches = await _db.Branches
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(
                x.Name, x.Id.ToString()))
            .ToListAsync();

        model.StaffMembers = await _db.StaffMembers
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem(
                x.FullName, x.Id.ToString()))
            .ToListAsync();

        model.HealthcareServices = await _db.HealthcareServices
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(
                x.Name, x.Id.ToString()))
            .ToListAsync();
    }
}
