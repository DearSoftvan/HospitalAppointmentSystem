namespace HospitalAppointmentSystem.Web.ViewModels;

public class StaffApplicationReviewViewModel
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string Profession { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public DateTime CreatedAt { get; set; }
}
