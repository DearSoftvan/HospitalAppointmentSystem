using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

public class Nurse : Staff
{
    public override Profession Profession => Profession.Nurse;

    private Nurse() { }

    public Nurse(int branchId, string fullName)
        : base(branchId, fullName)
    {
    }
}
