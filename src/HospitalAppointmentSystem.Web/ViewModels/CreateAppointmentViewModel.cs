using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class CreateAppointmentViewModel
{
    [Range(1, int.MaxValue)]
    [Display(Name = "Healthcare organization")]
    public int OrganizationId { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Branch")]
    public int BranchId { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Healthcare professional")]
    public int StaffId { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Healthcare service")]
    public int HealthcareServiceId { get; set; }

    [Required]
    [Display(Name = "Appointment date and time")]
    public DateTime? Start { get; set; }

    public IEnumerable<SelectListItem> Organizations { get; set; } = [];
    public IEnumerable<SelectListItem> Branches { get; set; } = [];
    public IEnumerable<SelectListItem> StaffMembers { get; set; } = [];
    public IEnumerable<SelectListItem> HealthcareServices { get; set; } = [];
}
