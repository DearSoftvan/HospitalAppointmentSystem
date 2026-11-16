using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.Application.Interfaces;

public interface IApprovalStrategy
{
    ApprovalMode Mode { get; }

    AppointmentStatus DetermineInitialStatus();
}
