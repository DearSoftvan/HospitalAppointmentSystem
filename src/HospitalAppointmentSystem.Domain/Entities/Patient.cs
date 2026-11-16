namespace HospitalAppointmentSystem.Domain.Entities;

public class Patient
{
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string? IdentityUserId { get; private set; }
    public bool IsActive { get; private set; }

    private Patient()
    {
        FullName = string.Empty;
    }

    public Patient(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException(
                "Patient name is required.", nameof(fullName));

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
