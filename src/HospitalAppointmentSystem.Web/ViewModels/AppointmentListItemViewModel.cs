namespace HospitalAppointmentSystem.Web.ViewModels;

public class AppointmentListItemViewModel
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
