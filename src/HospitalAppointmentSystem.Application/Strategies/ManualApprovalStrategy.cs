using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Application.Strategies;

public sealed class ManualApprovalStrategy : IApprovalStrategy
{
    public ApprovalMode Mode => ApprovalMode.Manual;

    public AppointmentStatus DetermineInitialStatus()
    {
        return AppointmentStatus.Pending;
    }
}
