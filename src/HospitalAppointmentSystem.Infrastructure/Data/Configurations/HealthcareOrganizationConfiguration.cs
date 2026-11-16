using HospitalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Data.Configurations;

public class HealthcareOrganizationConfiguration
    : IEntityTypeConfiguration<HealthcareOrganization>
{
    public void Configure(EntityTypeBuilder<HealthcareOrganization> builder)
    {
        builder.ToTable("HealthcareOrganizations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ApprovalMode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
