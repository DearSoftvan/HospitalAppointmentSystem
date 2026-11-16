using HospitalAppointmentSystem.Application.DTOs;
using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Application.Services;
using HospitalAppointmentSystem.Application.Strategies;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HospitalAppointmentSystem.IntegrationTests;

public class AppointmentWorkflowTests
{
    [Fact]
    public async Task Automatic_Approval_Should_Persist_Appointment_In_Sqlite()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"hospital-workflow-{Guid.NewGuid():N}.db");

        try
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={databasePath}"));

            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IHealthcareServiceRepository, HealthcareServiceRepository>();
            services.AddScoped<IStaffServiceRepository, StaffServiceRepository>();
            services.AddScoped<IStaffAvailabilityRepository, StaffAvailabilityRepository>();
            services.AddScoped<IBranchWorkingHoursRepository, BranchWorkingHoursRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();

            services.AddScoped<IApprovalStrategy, AutomaticApprovalStrategy>();
            services.AddScoped<IApprovalStrategy, ManualApprovalStrategy>();
            services.AddScoped<AppointmentService>();

            using var provider = services.BuildServiceProvider();

            int patientId;
            int staffId;
            int branchId;
            int serviceId;

            var start = DateTime.Today.AddDays(1).AddHours(10);
            var end = start.AddMinutes(30);

            // Arrange: Create actual records in SQLite.
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                await db.Database.EnsureCreatedAsync();

                var organization = new HealthcareOrganization(
                    "Integration Test Hospital",
                    ApprovalMode.Automatic);

                db.HealthcareOrganizations.Add(organization);
                await db.SaveChangesAsync();

                var branch = new Branch(
                    organization.Id,
                    "Central Branch",
                    "Test Address");

                db.Branches.Add(branch);
                await db.SaveChangesAsync();

                var patient = new Patient("Test Patient");

                var doctor = new Doctor(
                    branch.Id,
                    "Test Doctor",
                    "Cardiology");

                var healthcareService = new HealthcareService(
                    "Cardiology Examination",
                    30,
                    500m);

                db.Patients.Add(patient);
                db.StaffMembers.Add(doctor);
                db.HealthcareServices.Add(healthcareService);
                await db.SaveChangesAsync();

                db.StaffServices.Add(
                    new StaffService(doctor.Id, healthcareService.Id));

                db.WorkingHours.Add(
                    new WorkingHours(
                        branch.Id,
                        start.DayOfWeek,
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0)));

                db.StaffAvailabilities.Add(
                    new StaffAvailability(
                        doctor.Id,
                        start.Date.AddHours(9),
                        start.Date.AddHours(17)));

                await db.SaveChangesAsync();

                patientId = patient.Id;
                staffId = doctor.Id;
                branchId = branch.Id;
                serviceId = healthcareService.Id;
            }

            int appointmentId;

            // Act: Create appointment using real repositories.
            using (var scope = provider.CreateScope())
            {
                var appointmentService = scope.ServiceProvider
                    .GetRequiredService<AppointmentService>();

                var request = new CreateAppointmentRequest(
                    patientId,
                    staffId,
                    branchId,
                    serviceId,
                    start);

                var appointment =
                    await appointmentService.CreateAsync(request);

                Assert.Equal(AppointmentStatus.Approved, appointment.Status);
                Assert.Equal(500m, appointment.Price);
                Assert.Equal(end, appointment.End);

                appointmentId = appointment.Id;
                Assert.True(appointmentId > 0);
            }

            // Assert: Read from SQLite through a new DbContext.
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var persisted = await db.Appointments
                    .AsNoTracking()
                    .SingleAsync(x => x.Id == appointmentId);

                Assert.Equal(patientId, persisted.PatientId);
                Assert.Equal(staffId, persisted.StaffId);
                Assert.Equal(branchId, persisted.BranchId);
                Assert.Equal(serviceId, persisted.HealthcareServiceId);
                Assert.Equal(AppointmentStatus.Approved, persisted.Status);

                Assert.Equal(1, await db.Appointments.CountAsync());
            }
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (File.Exists(databasePath))
                File.Delete(databasePath);
        }
    }
}
