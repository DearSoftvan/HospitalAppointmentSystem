using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Data.Configurations;

public class DepartmentServiceConfiguration
    : IEntityTypeConfiguration<DepartmentService>
{
    public void Configure(
        EntityTypeBuilder<DepartmentService> builder)
    {
        builder.HasKey(x => new
        {
            x.DepartmentId,
            x.HealthcareServiceId
        });

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HealthcareService>()
            .WithMany()
            .HasForeignKey(x => x.HealthcareServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
