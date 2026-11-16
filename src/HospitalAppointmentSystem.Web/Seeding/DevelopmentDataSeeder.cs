using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Seeding;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // Do not insert the same demo data on every startup.
        if (await db.HealthcareOrganizations.AnyAsync(
            x => x.Name == "Demo General Hospital"))
        {
            return;
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync();

        // Organizations with different approval strategies.
        var automaticHospital = new HealthcareOrganization(
            "Demo General Hospital",
            ApprovalMode.Automatic);

        var manualHospital = new HealthcareOrganization(
            "Demo Medical Center",
            ApprovalMode.Manual);

        db.HealthcareOrganizations.AddRange(
            automaticHospital, manualHospital);

        await db.SaveChangesAsync();

        // Branches.
        var mainBranch = new Branch(
            automaticHospital.Id,
            "Central Branch",
            "Demo Street 10");

        var medicalBranch = new Branch(
            manualHospital.Id,
            "Medical Center Branch",
            "Demo Avenue 20");

        db.Branches.AddRange(mainBranch, medicalBranch);
        await db.SaveChangesAsync();

        // Healthcare professionals.
        var firstDoctor = new Doctor(
            mainBranch.Id,
            "Dr. Alice Smith",
            "Internal Medicine");

        var secondDoctor = new Doctor(
            medicalBranch.Id,
            "Dr. Daniel Brown",
            "Cardiology");

        db.StaffMembers.AddRange(firstDoctor, secondDoctor);

        // Healthcare services.
        var examination = new HealthcareService(
            "General Examination",
            30,
            500m);

        var cardiology = new HealthcareService(
            "Cardiology Consultation",
            45,
            750m);

        db.HealthcareServices.AddRange(
            examination, cardiology);

        await db.SaveChangesAsync();

        // Which professional can provide which service?
        db.StaffServices.AddRange(
            new StaffService(firstDoctor.Id, examination.Id),
            new StaffService(secondDoctor.Id, cardiology.Id));

        // Branch working hours: every day, 09:00-17:00.
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            db.WorkingHours.Add(
                new WorkingHours(
                    mainBranch.Id,
                    day,
                    new TimeOnly(9, 0),
                    new TimeOnly(17, 0)));

            db.WorkingHours.Add(
                new WorkingHours(
                    medicalBranch.Id,
                    day,
                    new TimeOnly(9, 0),
                    new TimeOnly(17, 0)));
        }

        // Staff availability for the next 60 days.
        for (var day = 1; day <= 60; day++)
        {
            var date = DateTime.Today.AddDays(day);

            var start = date.AddHours(9);
            var end = date.AddHours(17);

            db.StaffAvailabilities.Add(
                new StaffAvailability(
                    firstDoctor.Id, start, end));

            db.StaffAvailabilities.Add(
                new StaffAvailability(
                    secondDoctor.Id, start, end));
        }

        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
