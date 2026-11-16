using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

public class HealthcareOrganization
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public ApprovalMode ApprovalMode { get; private set; }
    public bool IsActive { get; private set; }

    private HealthcareOrganization()
    {
        Name = string.Empty;
    }

    public HealthcareOrganization(
        string name,
        ApprovalMode approvalMode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Organization name cannot be empty.",
                nameof(name));

        if (!Enum.IsDefined(approvalMode))
            throw new ArgumentOutOfRangeException(
                nameof(approvalMode));

        Name = name.Trim();
        ApprovalMode = approvalMode;
        IsActive = true;
    }

    public void ChangeApprovalMode(ApprovalMode mode)
    {
        if (!IsActive)
            throw new InvalidOperationException(
                "Inactive organizations cannot be modified.");

        if (!Enum.IsDefined(mode))
            throw new ArgumentOutOfRangeException(
                nameof(mode));

        ApprovalMode = mode;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
