using HospitalAppointmentSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace HospitalAppointmentSystem.Web.Seeding;

public static class DevelopmentAdminSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            services.GetRequiredService<UserManager<ApplicationUser>>();

        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole(adminRole));

            if (!roleResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ",
                        roleResult.Errors.Select(x => x.Description)));
        }

        var email = Environment.GetEnvironmentVariable(
            "HOSPITAL_ADMIN_EMAIL");

        var password = Environment.GetEnvironmentVariable(
            "HOSPITAL_ADMIN_PASSWORD");

        // Admin account creation is opt-in.
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        email = email.Trim();

        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            if (!await userManager.IsInRoleAsync(
                existingUser, adminRole))
            {
                throw new InvalidOperationException(
                    "The configured admin email already belongs " +
                    "to a non-admin account. Use a different email.");
            }

            return;
        }

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = "Development Administrator"
        };

        var createResult = await userManager.CreateAsync(
            admin, password);

        if (!createResult.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ",
                    createResult.Errors.Select(x => x.Description)));

        var addRoleResult = await userManager.AddToRoleAsync(
            admin, adminRole);

        if (!addRoleResult.Succeeded)
            throw new InvalidOperationException(
                string.Join("; ",
                    addRoleResult.Errors.Select(x => x.Description)));

        Console.WriteLine("Development admin account is ready.");
    }
}
