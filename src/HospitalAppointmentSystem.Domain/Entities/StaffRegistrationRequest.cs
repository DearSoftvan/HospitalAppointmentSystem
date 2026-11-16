using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Domain.Entities;

public class StaffRegistrationRequest
{
    public int Id { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public int BranchId { get; private set; }
    public Profession Profession { get; private set; }
    public string? Specialty { get; private set; }
    public StaffRegistrationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewedByUserId { get; private set; }

    private StaffRegistrationRequest() { }

    public StaffRegistrationRequest(
        string identityUserId,
        int branchId,
        Profession profession,
        string? specialty)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            throw new ArgumentException(
                "Identity user ID is required.", nameof(identityUserId));

        if (branchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(branchId));

        if (!Enum.IsDefined(profession))
            throw new ArgumentOutOfRangeException(nameof(profession));

        if (profession == Profession.Doctor &&
            string.IsNullOrWhiteSpace(specialty))
            throw new ArgumentException(
                "Doctor specialty is required.", nameof(specialty));

        if (specialty?.Trim().Length > 200)
            throw new ArgumentException(
                "Specialty cannot exceed 200 characters.",
                nameof(specialty));

        IdentityUserId = identityUserId.Trim();
        BranchId = branchId;
        Profession = profession;
        Specialty = specialty?.Trim();
        Status = StaffRegistrationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve(string reviewerUserId)
    {
        EnsurePending();
        SetReview(StaffRegistrationStatus.Approved, reviewerUserId);
    }

    public void Reject(string reviewerUserId)
    {
        EnsurePending();
        SetReview(StaffRegistrationStatus.Rejected, reviewerUserId);
    }

    private void EnsurePending()
    {
        if (Status != StaffRegistrationStatus.Pending)
            throw new InvalidOperationException(
                "Only pending applications can be reviewed.");
    }

    private void SetReview(
        StaffRegistrationStatus status,
        string reviewerUserId)
    {
        if (string.IsNullOrWhiteSpace(reviewerUserId))
            throw new ArgumentException(
                "Reviewer ID is required.", nameof(reviewerUserId));

        Status = status;
        ReviewedAt = DateTime.UtcNow;
        ReviewedByUserId = reviewerUserId.Trim();
    }
}
