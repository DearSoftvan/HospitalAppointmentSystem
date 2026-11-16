namespace HospitalAppointmentSystem.Web.ViewModels;

public class StaffAppointmentViewModel
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Status { get; set; } = string.Empty;
}
