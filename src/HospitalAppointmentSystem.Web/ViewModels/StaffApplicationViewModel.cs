using System.ComponentModel.DataAnnotations;
using HospitalAppointmentSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class StaffApplicationViewModel
{
    [Required]
    [StringLength(200)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Display(Name = "Healthcare branch")]
    public int BranchId { get; set; }

    [Required]
    [Display(Name = "Profession")]
    public Profession Profession { get; set; } = Profession.Doctor;

    [StringLength(200)]
    [Display(Name = "Specialty (required for doctors)")]
    public string? Specialty { get; set; }

    public IEnumerable<SelectListItem> Branches { get; set; } = [];
}
