using Microsoft.AspNetCore.Identity;

namespace HospitalAppointmentSystem.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
