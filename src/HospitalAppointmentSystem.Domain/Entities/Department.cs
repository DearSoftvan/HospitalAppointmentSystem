namespace HospitalAppointmentSystem.Domain.Entities;

public class Department
{
    public int Id { get; private set; }
    public int BranchId { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    private Department()
    {
        Name = string.Empty;
    }

    public Department(int branchId, string name)
    {
        if (branchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(branchId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required.");

        BranchId = branchId;
        Name = name.Trim();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
