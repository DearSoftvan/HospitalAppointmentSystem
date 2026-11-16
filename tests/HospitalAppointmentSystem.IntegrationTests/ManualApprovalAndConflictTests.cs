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

public class ManualApprovalAndConflictTests
{
    [Fact]
    public async Task Manual_Appointment_Should_Be_Pending_And_Block_Overlapping_Request()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"hospital-manual-{Guid.NewGuid():N}.db");

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
            int secondPatientId;
            int staffId;
            int branchId;
            int healthcareServiceId;

            var start = DateTime.Today.AddDays(1).AddHours(10);

            // Arrange: Seed a separate SQLite database.
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                await db.Database.EnsureCreatedAsync();

                var organization = new HealthcareOrganization(
                    "Manual Approval Hospital",
                    ApprovalMode.Manual);

                db.HealthcareOrganizations.Add(organization);
                await db.SaveChangesAsync();

                var branch = new Branch(
                    organization.Id,
                    "Main Branch",
                    "Test Address");

                db.Branches.Add(branch);
                await db.SaveChangesAsync();

                var patient = new Patient("First Patient");
                var secondPatient = new Patient("Second Patient");

                var doctor = new Doctor(
                    branch.Id,
                    "Test Doctor",
                    "Cardiology");

                var healthcareService = new HealthcareService(
                    "Cardiology Examination",
                    30,
                    500m);

                db.Patients.AddRange(patient, secondPatient);
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
                secondPatientId = secondPatient.Id;
                staffId = doctor.Id;
                branchId = branch.Id;
                healthcareServiceId = healthcareService.Id;
            }

            // Act: Create the first appointment.
            using (var scope = provider.CreateScope())
            {
                var appointmentService = scope.ServiceProvider
                    .GetRequiredService<AppointmentService>();

                var first = await appointmentService.CreateAsync(
                    new CreateAppointmentRequest(
                        patientId,
                        staffId,
                        branchId,
                        healthcareServiceId,
                        start));

                Assert.Equal(AppointmentStatus.Pending, first.Status);
                Assert.True(first.Id > 0);
            }

            // Act: Another patient requests the same doctor and time.
            using (var scope = provider.CreateScope())
            {
                var appointmentService = scope.ServiceProvider
                    .GetRequiredService<AppointmentService>();

                await Assert.ThrowsAnyAsync<Exception>(
                    async () => await appointmentService.CreateAsync(
                        new CreateAppointmentRequest(
                            secondPatientId,
                            staffId,
                            branchId,
                            healthcareServiceId,
                            start)));
            }

            // Assert: Only the original Pending appointment exists.
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var appointments = await db.Appointments
                    .AsNoTracking()
                    .ToListAsync();

                Assert.Single(appointments);
                Assert.Equal(
                    AppointmentStatus.Pending,
                    appointments[0].Status);

                Assert.Equal(patientId, appointments[0].PatientId);
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
