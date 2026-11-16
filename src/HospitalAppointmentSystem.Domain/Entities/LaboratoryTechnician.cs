using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

public class LaboratoryTechnician : Staff
{
    public override Profession Profession =>
        Profession.LaboratoryTechnician;

    private LaboratoryTechnician() { }

    public LaboratoryTechnician(int branchId, string fullName)
        : base(branchId, fullName)
    {
    }
}
