namespace HospitalAppointmentSystem.Web.ViewModels;

public class AppointmentHistoryViewModel
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentStart { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;

    public List<AppointmentHistoryItemViewModel> Events { get; set; } = [];
}

public class AppointmentHistoryItemViewModel
{
    public string Status { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? Note { get; set; }
}
