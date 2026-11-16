using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HospitalAppointmentSystem.IntegrationTests;

public class DatabaseMappingTests
{
    [Fact]
    public async Task Database_Should_Create_Tables_And_Staff_Discriminator()
    {
        var databasePath = Path.Combine(
            Path.GetTempPath(),
            $"hospital-test-{Guid.NewGuid():N}.db");

        try
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            await using (var context = new ApplicationDbContext(options))
            {
                await context.Database.EnsureCreatedAsync();
            }

            await using var connection = new SqliteConnection(
                $"Data Source={databasePath}");

            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = """
                SELECT COUNT(*)
                FROM sqlite_master
                WHERE type = 'table'
                  AND name IN ('Staff', 'Appointments', 'Patients', 'AspNetUsers');
                """;

            var tableCount = Convert.ToInt32(
                await command.ExecuteScalarAsync());

            Assert.Equal(4, tableCount);

            command.CommandText = """
                SELECT COUNT(*)
                FROM pragma_table_info('Staff')
                WHERE name = 'StaffType';
                """;

            var discriminatorCount = Convert.ToInt32(
                await command.ExecuteScalarAsync());

            Assert.Equal(1, discriminatorCount);
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (File.Exists(databasePath))
                File.Delete(databasePath);
        }
    }
}
