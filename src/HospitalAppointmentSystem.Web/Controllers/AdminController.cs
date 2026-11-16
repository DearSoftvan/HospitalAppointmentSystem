using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var appointments = await (
            from appointment in _db.Appointments.AsNoTracking()
            join patient in _db.Patients
                on appointment.PatientId equals patient.Id
            join staff in _db.StaffMembers
                on appointment.StaffId equals staff.Id
            join branch in _db.Branches
                on appointment.BranchId equals branch.Id
            join organization in _db.HealthcareOrganizations
                on branch.OrganizationId equals organization.Id
            join service in _db.HealthcareServices
                on appointment.HealthcareServiceId equals service.Id
            where appointment.Status == AppointmentStatus.Pending
            orderby appointment.Start
            select new PendingAppointmentViewModel
            {
                Id = appointment.Id,
                Start = appointment.Start,
                PatientName = patient.FullName,
                StaffName = staff.FullName,
                OrganizationName = organization.Name,
                BranchName = branch.Name,
                ServiceName = service.Name
            }).ToListAsync();

        return View(appointments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (appointment == null)
            return NotFound();

        if (appointment.Status != AppointmentStatus.Pending)
        {
            TempData["ErrorMessage"] =
                "Only pending appointments can be approved.";

            return RedirectToAction(nameof(Index));
        }

        appointment.Approve();
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"Appointment #{id} approved.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (appointment == null)
            return NotFound();

        if (appointment.Status != AppointmentStatus.Pending)
        {
            TempData["ErrorMessage"] =
                "Only pending appointments can be rejected.";

            return RedirectToAction(nameof(Index));
        }

        appointment.Reject();
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"Appointment #{id} rejected.";

        return RedirectToAction(nameof(Index));
    }
}
