using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalAppointmentSystem.Infrastructure.Data.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staff");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasDiscriminator<string>("StaffType")
            .HasValue<Doctor>("Doctor")
            .HasValue<Nurse>("Nurse")
            .HasValue<LaboratoryTechnician>("LaboratoryTechnician");

        builder.Ignore(x => x.Profession);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.IdentityUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.IdentityUserId)
            .IsUnique();
    }
}
