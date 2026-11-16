using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Infrastructure.Repositories;
using HospitalAppointmentSystem.Application.Services;
using HospitalAppointmentSystem.Application.Strategies;

var builder = WebApplication.CreateBuilder(args);

// MVC services
builder.Services.AddControllersWithViews();

// SQLite database configuration
var solutionRoot = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "../../"));

var databaseDirectory = Path.Combine(solutionRoot, "data");

Directory.CreateDirectory(databaseDirectory);

var databasePath = Path.Combine(
    databaseDirectory,
    "hospital.db");

var connectionString = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
{
    DataSource = databasePath
}.ToString();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// ASP.NET Core Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// Repository registrations
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IHealthcareServiceRepository, HealthcareServiceRepository>();

builder.Services.AddScoped<IStaffServiceRepository, StaffServiceRepository>();
builder.Services.AddScoped<IStaffAvailabilityRepository, StaffAvailabilityRepository>();
builder.Services.AddScoped<IBranchWorkingHoursRepository, BranchWorkingHoursRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// Appointment application service
builder.Services.AddScoped<AppointmentService>();

// Appointment approval strategies
builder.Services.AddScoped<IApprovalStrategy, AutomaticApprovalStrategy>();
builder.Services.AddScoped<IApprovalStrategy, ManualApprovalStrategy>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
