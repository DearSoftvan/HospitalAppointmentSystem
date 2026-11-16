namespace HospitalAppointmentSystem.Domain.Entities;

public class HealthcareService
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }

    private HealthcareService()
    {
        Name = string.Empty;
    }

    public HealthcareService(
        string name,
        int durationMinutes,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Service name is required.");

        if (durationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price));

        Name = name.Trim();
        DurationMinutes = durationMinutes;
        Price = price;
        IsActive = true;
    }

    public void UpdateDetails(int durationMinutes, decimal price)
    {
        if (durationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price));

        DurationMinutes = durationMinutes;
        Price = price;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
