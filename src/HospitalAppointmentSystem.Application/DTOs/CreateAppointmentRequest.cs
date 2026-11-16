namespace HospitalAppointmentSystem.Application.DTOs;

public sealed record CreateAppointmentRequest(
    int PatientId,
    int StaffId,
    int BranchId,
    int HealthcareServiceId,
    DateTime Start
);
