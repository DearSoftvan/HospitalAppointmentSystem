using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Data.Configurations;

public class StaffAvailabilityConfiguration
    : IEntityTypeConfiguration<StaffAvailability>
{
    public void Configure(EntityTypeBuilder<StaffAvailability> builder)
    {
        builder.ToTable("StaffAvailabilities");
        builder.HasKey(x => x.Id);

        builder.HasOne<Staff>()
            .WithMany()
            .HasForeignKey(x => x.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.StaffId, x.Start, x.End });
    }
}
