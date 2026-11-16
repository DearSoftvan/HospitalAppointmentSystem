using System.Security.Claims;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Infrastructure.Identity;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

public class StaffApplyController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public StaffApplyController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Pending));

        var model = new StaffApplicationViewModel();
        await LoadBranchesAsync(model);
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        StaffApplicationViewModel model)
    {
        if (User.Identity?.IsAuthenticated == true)
            return BadRequest("Please sign out before creating a new account.");

        if (!Enum.IsDefined(model.Profession))
        {
            ModelState.AddModelError(
                nameof(model.Profession), "Invalid profession.");
        }

        if (model.Profession == Profession.Doctor &&
            string.IsNullOrWhiteSpace(model.Specialty))
        {
            ModelState.AddModelError(
                nameof(model.Specialty),
                "Specialty is required for doctors.");
        }

        var branchExists = await _db.Branches
            .AnyAsync(x => x.Id == model.BranchId && x.IsActive);

        if (!branchExists)
        {
            ModelState.AddModelError(
                nameof(model.BranchId),
                "Please select an active healthcare branch.");
        }

        if (!ModelState.IsValid)
        {
            await LoadBranchesAsync(model);
            return View(model);
        }

        var email = model.Email.Trim();

        if (await _userManager.FindByEmailAsync(email) != null)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "This email is already registered.");

            await LoadBranchesAsync(model);
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = model.FullName.Trim()
        };

        await using (var transaction =
            await _db.Database.BeginTransactionAsync())
        {
            var result = await _userManager.CreateAsync(
                user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(
                        string.Empty, error.Description);

                await LoadBranchesAsync(model);
                return View(model);
            }

            var application = new StaffRegistrationRequest(
                user.Id,
                model.BranchId,
                model.Profession,
                model.Specialty);

            _db.StaffRegistrationRequests.Add(application);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        // No Staff role is granted here.
        await _signInManager.SignInAsync(
            user, isPersistent: false);

        return RedirectToAction(nameof(Pending));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Pending()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var requests = await _db.StaffRegistrationRequests
            .AsNoTracking()
            .Where(x => x.IdentityUserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var model = requests.Select(x =>
            new StaffApplicationStatusViewModel
            {
                Id = x.Id,
                BranchId = x.BranchId,
                Profession = x.Profession.ToString(),
                Specialty = x.Specialty,
                Status = x.Status.ToString()
            }).ToList();

        return View(model);
    }

    private async Task LoadBranchesAsync(
        StaffApplicationViewModel model)
    {
        model.Branches = await _db.Branches
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToListAsync();
    }
}
