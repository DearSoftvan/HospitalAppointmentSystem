using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Application.Strategies;

public sealed class AutomaticApprovalStrategy : IApprovalStrategy
{
    public ApprovalMode Mode => ApprovalMode.Automatic;

    public AppointmentStatus DetermineInitialStatus()
    {
        return AppointmentStatus.Approved;
    }
}
