using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Infrastructure.Identity;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class StaffAccountsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public StaffAccountsController(
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
        await LoadStaffAsync();
        return View(new CreateStaffAccountViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        CreateStaffAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadStaffAsync();
            return View(model);
        }

        var staff = await _db.StaffMembers
            .FirstOrDefaultAsync(x =>
                x.Id == model.StaffId && x.IsActive);

        if (staff == null || staff.IdentityUserId != null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Staff not found or already has an account.");
            await LoadStaffAsync();
            return View(model);
        }

        var email = model.Email.Trim();

        if (await _userManager.FindByEmailAsync(email) != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "This email address is already in use.");
            await LoadStaffAsync();
            return View(model);
        }

        if (!await _roleManager.RoleExistsAsync("Staff"))
        {
            var roleResult = await _roleManager.CreateAsync(
                new IdentityRole("Staff"));

            if (!roleResult.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the Staff role.");
                await LoadStaffAsync();
                return View(model);
            }
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = staff.FullName
        };

        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var created = await _userManager.CreateAsync(
            user, model.Password);

        if (!created.Succeeded)
        {
            foreach (var error in created.Errors)
                ModelState.AddModelError(
                    string.Empty, error.Description);

            await LoadStaffAsync();
            return View(model);
        }

        var assigned = await _userManager.AddToRoleAsync(
            user, "Staff");

        if (!assigned.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to assign the Staff role.");
            await LoadStaffAsync();
            return View(model);
        }

        staff.LinkIdentityUser(user.Id);

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["SuccessMessage"] =
            "Staff account created successfully.";

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadStaffAsync()
    {
        var availableStaff = await _db.StaffMembers
            .AsNoTracking()
            .Where(x => x.IsActive && x.IdentityUserId == null)
            .OrderBy(x => x.FullName)
            .ToListAsync();

        ViewBag.StaffOptions = availableStaff
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.FullName} ({x.Profession})"
            })
            .ToList();
    }
}
