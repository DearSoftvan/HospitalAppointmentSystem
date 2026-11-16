namespace HospitalAppointmentSystem.Web.ViewModels;

public class StaffApplicationStatusViewModel
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string Profession { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public string Status { get; set; } = string.Empty;
}
