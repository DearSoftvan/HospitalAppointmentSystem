using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Data.Configurations;

public class StaffRegistrationRequestConfiguration
    : IEntityTypeConfiguration<StaffRegistrationRequest>
{
    public void Configure(
        EntityTypeBuilder<StaffRegistrationRequest> builder)
    {
        builder.ToTable("StaffRegistrationRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IdentityUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.Profession)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Specialty)
            .HasMaxLength(200);

        builder.Property(x => x.ReviewedByUserId)
            .HasMaxLength(450);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.IdentityUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.IdentityUserId,
            x.Status
        });

        builder.HasIndex(x => new
        {
            x.BranchId,
            x.Status
        });
    }
}
