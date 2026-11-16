namespace HospitalAppointmentSystem.Web.ViewModels;

public class PendingAppointmentViewModel
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string StaffName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
}
