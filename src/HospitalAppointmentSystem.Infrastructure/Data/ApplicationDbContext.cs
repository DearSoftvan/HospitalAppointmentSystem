using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<HealthcareOrganization> HealthcareOrganizations
        => Set<HealthcareOrganization>();

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<HealthcareService> HealthcareServices
        => Set<HealthcareService>();

    public DbSet<DepartmentService> DepartmentServices
        => Set<DepartmentService>();

    public DbSet<WorkingHours> WorkingHours
        => Set<WorkingHours>();

    public DbSet<Staff> StaffMembers => Set<Staff>();
    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<StaffService> StaffServices
        => Set<StaffService>();

    public DbSet<StaffAvailability> StaffAvailabilities
        => Set<StaffAvailability>();

    public DbSet<Appointment> Appointments
        => Set<Appointment>();

    public DbSet<AppointmentStatusHistory> AppointmentStatusHistories
        => Set<AppointmentStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
