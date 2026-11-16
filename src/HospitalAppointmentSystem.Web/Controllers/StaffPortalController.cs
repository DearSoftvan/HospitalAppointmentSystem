using System.Security.Claims;
using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Staff")]
public class StaffPortalController : Controller
{
    private readonly ApplicationDbContext _db;

    public StaffPortalController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var staff = await _db.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.IdentityUserId == userId && x.IsActive);

        if (staff == null)
            return Forbid();

        ViewBag.StaffName = staff.FullName;
        ViewBag.Profession = staff.Profession.ToString();

        var appointments = await (
            from appointment in _db.Appointments.AsNoTracking()
            join patient in _db.Patients
                on appointment.PatientId equals patient.Id
            join service in _db.HealthcareServices
                on appointment.HealthcareServiceId equals service.Id
            where appointment.StaffId == staff.Id
            orderby appointment.Start
            select new StaffAppointmentViewModel
            {
                Id = appointment.Id,
                PatientName = patient.FullName,
                ServiceName = service.Name,
                Start = appointment.Start,
                End = appointment.End,
                Status = appointment.Status.ToString()
            }).ToListAsync();

        return View(appointments);
    }
}
