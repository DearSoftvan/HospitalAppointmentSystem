using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Data.Configurations;

public class HealthcareServiceConfiguration
    : IEntityTypeConfiguration<HealthcareService>
{
    public void Configure(EntityTypeBuilder<HealthcareService> builder)
    {
        builder.ToTable("HealthcareServices");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.DurationMinutes)
            .IsRequired();
    }
}
