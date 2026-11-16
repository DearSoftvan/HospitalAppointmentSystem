namespace HospitalAppointmentSystem.Domain.Entities;

public class Branch
{
    public int Id { get; private set; }
    public int OrganizationId { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public bool IsActive { get; private set; }

    private Branch()
    {
        Name = string.Empty;
        Address = string.Empty;
    }

    public Branch(int organizationId, string name, string address)
    {
        if (organizationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(organizationId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Branch name is required.");

        Name = name.Trim();
        Address = address?.Trim() ?? string.Empty;
        OrganizationId = organizationId;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
