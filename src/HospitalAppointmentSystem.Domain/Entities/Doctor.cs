using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

public class Doctor : Staff
{
    public string Specialty { get; private set; }

    public override Profession Profession => Profession.Doctor;

    private Doctor()
    {
        Specialty = string.Empty;
    }

    public Doctor(int branchId, string fullName, string specialty)
        : base(branchId, fullName)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            throw new ArgumentException(
                "Specialty is required.", nameof(specialty));

        Specialty = specialty.Trim();
    }
}
