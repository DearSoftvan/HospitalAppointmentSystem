using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using HospitalAppointmentSystem.Infrastructure.Data;
using HospitalAppointmentSystem.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HospitalAppointmentSystem.IntegrationTests;

public class RepositoryPersistenceTests
{
    [Fact]
    public async Task Organization_And_Branch_Should_Persist_And_Be_Retrieved()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"hospital-repository-test-{Guid.NewGuid():N}.db");

        try
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            int organizationId;
            int branchId;

            // Arrange: Create test data in SQLite.
            await using (var context = new ApplicationDbContext(options))
            {
                await context.Database.EnsureCreatedAsync();

                var organization = new HealthcareOrganization(
                    "Test Hospital",
                    ApprovalMode.Automatic);

                context.HealthcareOrganizations.Add(organization);
                await context.SaveChangesAsync();

                var branch = new Branch(
                    organization.Id,
                    "Central Branch",
                    "Test Address");

                context.Branches.Add(branch);
                await context.SaveChangesAsync();

                organizationId = organization.Id;
                branchId = branch.Id;
            }

            // Act: Read through real repositories using a new DbContext.
            await using (var context = new ApplicationDbContext(options))
            {
                var organizationRepository =
                    new OrganizationRepository(context);

                var branchRepository =
                    new BranchRepository(context);

                var organization = await organizationRepository
                    .GetByIdAsync(organizationId);

                var branch = await branchRepository
                    .GetByIdAsync(branchId);

                // Assert
                Assert.NotNull(organization);
                Assert.NotNull(branch);

                Assert.Equal("Test Hospital", organization.Name);
                Assert.Equal(ApprovalMode.Automatic, organization.ApprovalMode);

                Assert.Equal(organizationId, branch.OrganizationId);
                Assert.Equal("Central Branch", branch.Name);
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
