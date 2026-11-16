using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Data.Configurations;

public class StaffServiceConfiguration
    : IEntityTypeConfiguration<StaffService>
{
    public void Configure(
        EntityTypeBuilder<StaffService> builder)
    {
        builder.HasKey(x => new
        {
            x.StaffId,
            x.HealthcareServiceId
        });

        builder.HasOne<Staff>()
            .WithMany()
            .HasForeignKey(x => x.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HealthcareService>()
            .WithMany()
            .HasForeignKey(x => x.HealthcareServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
