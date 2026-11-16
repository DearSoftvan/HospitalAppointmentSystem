using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.UnitTests.Domain;

public class HealthcareOrganizationTests
{
    [Fact]
    public void Constructor_ShouldCreateActiveOrganization()
    {
        var organization = new HealthcareOrganization(
            "Central Hospital",
            ApprovalMode.Manual);

        Assert.Equal("Central Hospital", organization.Name);
        Assert.Equal(ApprovalMode.Manual, organization.ApprovalMode);
        Assert.True(organization.IsActive);
    }

    [Fact]
    public void ChangeApprovalMode_ShouldUpdateMode()
    {
        var organization = new HealthcareOrganization(
            "Central Hospital",
            ApprovalMode.Manual);

        organization.ChangeApprovalMode(ApprovalMode.Automatic);

        Assert.Equal(
            ApprovalMode.Automatic,
            organization.ApprovalMode);
    }

    [Fact]
    public void ChangeApprovalMode_ShouldFailForInactiveOrganization()
    {
        var organization = new HealthcareOrganization(
            "Central Hospital",
            ApprovalMode.Manual);

        organization.Deactivate();

        Assert.Throws<InvalidOperationException>(() =>
            organization.ChangeApprovalMode(ApprovalMode.Automatic));
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyName()
    {
        Assert.Throws<ArgumentException>(() =>
            new HealthcareOrganization(
                "",
                ApprovalMode.Manual));
    }
}
