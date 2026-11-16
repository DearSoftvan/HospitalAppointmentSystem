using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

public abstract class Staff
{
    public int Id { get; private set; }
    public int BranchId { get; private set; }
    public string FullName { get; private set; }
    public string? IdentityUserId { get; private set; }
    public bool IsActive { get; private set; }

    public abstract Profession Profession { get; }

    protected Staff()
    {
        FullName = string.Empty;
    }

    protected Staff(int branchId, string fullName)
    {
        if (branchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(branchId));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException(
                "Staff name is required.", nameof(fullName));

        BranchId = branchId;
        FullName = fullName.Trim();
        IsActive = true;
    }

    public void LinkIdentityUser(string identityUserId)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            throw new ArgumentException(
                "Identity user ID is required.", nameof(identityUserId));

        IdentityUserId = identityUserId;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
