using System.Security.Claims;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Infrastructure.Identity;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class StaffApplicationsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public StaffApplicationsController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var requests = await _db.StaffRegistrationRequests
            .AsNoTracking()
            .Where(x => x.Status == StaffRegistrationStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        var userIds = requests.Select(x => x.IdentityUserId).ToList();
        var branchIds = requests.Select(x => x.BranchId).ToList();

        var users = await _db.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var branches = await _db.Branches
            .AsNoTracking()
            .Where(x => branchIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var model = requests.Select(x =>
        {
            users.TryGetValue(x.IdentityUserId, out var user);
            branches.TryGetValue(x.BranchId, out var branch);

            return new StaffApplicationReviewViewModel
            {
                Id = x.Id,
                ApplicantName = user?.FullName ?? "Unknown",
                Email = user?.Email ?? "",
                BranchName = branch?.Name ?? "Unknown",
                Profession = x.Profession.ToString(),
                Specialty = x.Specialty,
                CreatedAt = x.CreatedAt
            };
        }).ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var request = await _db.StaffRegistrationRequests
            .FirstOrDefaultAsync(x => x.Id == id);

        if (request == null)
            return NotFound();

        if (request.Status != StaffRegistrationStatus.Pending)
        {
            TempData["ErrorMessage"] = "Application already reviewed.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(request.IdentityUserId);
        var branch = await _db.Branches
            .FirstOrDefaultAsync(x => x.Id == request.BranchId);

        if (user == null || branch == null || !branch.IsActive)
        {
            TempData["ErrorMessage"] =
                "Applicant or active branch not found.";
            return RedirectToAction(nameof(Index));
        }

        var alreadyLinked = await _db.StaffMembers
            .AnyAsync(x => x.IdentityUserId == user.Id);

        if (alreadyLinked ||
            await _userManager.IsInRoleAsync(user, "Admin") ||
            await _userManager.IsInRoleAsync(user, "Staff"))
        {
            TempData["ErrorMessage"] =
                "Applicant already has a privileged or linked account.";
            return RedirectToAction(nameof(Index));
        }

        Staff staff;

        switch (request.Profession)
        {
            case Profession.Doctor:
                staff = new Doctor(
                    request.BranchId,
                    user.FullName,
                    request.Specialty!);
                break;

            case Profession.Nurse:
                staff = new Nurse(request.BranchId, user.FullName);
                break;

            case Profession.LaboratoryTechnician:
                staff = new LaboratoryTechnician(
                    request.BranchId, user.FullName);
                break;

            default:
                return BadRequest("Invalid profession.");
        }

        if (!await _roleManager.RoleExistsAsync("Staff"))
        {
            var roleResult = await _roleManager.CreateAsync(
                new IdentityRole("Staff"));

            if (!roleResult.Succeeded)
            {
                TempData["ErrorMessage"] =
                    "Unable to create Staff role.";
                return RedirectToAction(nameof(Index));
            }
        }

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var assigned = await _userManager.AddToRoleAsync(user, "Staff");

        if (!assigned.Succeeded)
        {
            TempData["ErrorMessage"] =
                string.Join("; ",
                    assigned.Errors.Select(x => x.Description));

            return RedirectToAction(nameof(Index));
        }

        staff.LinkIdentityUser(user.Id);

        _db.StaffMembers.Add(staff);

        var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException(
                "Administrator identity not found.");

        request.Approve(reviewerId);

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["SuccessMessage"] =
            "Staff application approved and account activated.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var request = await _db.StaffRegistrationRequests
            .FirstOrDefaultAsync(x => x.Id == id);

        if (request == null)
            return NotFound();

        if (request.Status != StaffRegistrationStatus.Pending)
        {
            TempData["ErrorMessage"] = "Application already reviewed.";
            return RedirectToAction(nameof(Index));
        }

        var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException(
                "Administrator identity not found.");

        request.Reject(reviewerId);

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "Staff application rejected.";

        return RedirectToAction(nameof(Index));
    }
}
