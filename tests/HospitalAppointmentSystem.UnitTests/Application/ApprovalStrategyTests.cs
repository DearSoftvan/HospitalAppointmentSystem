using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Application.Strategies;
using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.UnitTests.Application;

public class ApprovalStrategyTests
{
    [Fact]
    public void AutomaticStrategy_ShouldApproveAppointment()
    {
        IApprovalStrategy strategy =
            new AutomaticApprovalStrategy();

        Assert.Equal(ApprovalMode.Automatic, strategy.Mode);

        Assert.Equal(
            AppointmentStatus.Approved,
            strategy.DetermineInitialStatus());
    }

    [Fact]
    public void ManualStrategy_ShouldCreatePendingAppointment()
    {
        IApprovalStrategy strategy =
            new ManualApprovalStrategy();

        Assert.Equal(ApprovalMode.Manual, strategy.Mode);

        Assert.Equal(
            AppointmentStatus.Pending,
            strategy.DetermineInitialStatus());
    }
}
