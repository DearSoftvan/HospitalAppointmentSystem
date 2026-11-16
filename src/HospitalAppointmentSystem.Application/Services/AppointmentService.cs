using HospitalAppointmentSystem.Application.DTOs;
using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.ValueObjects;

namespace HospitalAppointmentSystem.Application.Services;

public sealed class AppointmentService
{
    private readonly IPatientRepository _patients;
    private readonly IStaffRepository _staff;
    private readonly IBranchRepository _branches;
    private readonly IOrganizationRepository _organizations;
    private readonly IHealthcareServiceRepository _services;
    private readonly IStaffServiceRepository _staffServices;
    private readonly IStaffAvailabilityRepository _availability;
    private readonly IBranchWorkingHoursRepository _workingHours;
    private readonly IAppointmentRepository _appointments;
    private readonly IReadOnlyCollection<IApprovalStrategy> _strategies;

    public AppointmentService(
        IPatientRepository patients,
        IStaffRepository staff,
        IBranchRepository branches,
        IOrganizationRepository organizations,
        IHealthcareServiceRepository services,
        IStaffServiceRepository staffServices,
        IStaffAvailabilityRepository availability,
        IBranchWorkingHoursRepository workingHours,
        IAppointmentRepository appointments,
        IEnumerable<IApprovalStrategy> strategies)
    {
        _patients = patients;
        _staff = staff;
        _branches = branches;
        _organizations = organizations;
        _services = services;
        _staffServices = staffServices;
        _availability = availability;
        _workingHours = workingHours;
        _appointments = appointments;
        _strategies = strategies.ToArray();
    }

    public async Task<Appointment> CreateAsync(
        CreateAppointmentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // 1. Validate patient.
        var patient = await _patients.GetByIdAsync(request.PatientId);

        if (patient is null || !patient.IsActive)
            throw new InvalidOperationException(
                "Patient does not exist or is inactive.");

        // 2. Validate branch.
        var branch = await _branches.GetByIdAsync(request.BranchId);

        if (branch is null || !branch.IsActive)
            throw new InvalidOperationException(
                "Branch does not exist or is inactive.");

        // 3. Validate healthcare organization.
        var organization = await _organizations.GetByIdAsync(
            branch.OrganizationId);

        if (organization is null || !organization.IsActive)
            throw new InvalidOperationException(
                "Healthcare organization is unavailable.");

        // 4. Validate staff.
        var staff = await _staff.GetByIdAsync(request.StaffId);

        if (staff is null || !staff.IsActive)
            throw new InvalidOperationException(
                "Staff member does not exist or is inactive.");

        if (staff.BranchId != branch.Id)
            throw new InvalidOperationException(
                "Staff member does not belong to this branch.");

        // 5. Validate healthcare service.
        var service = await _services.GetByIdAsync(
            request.HealthcareServiceId);

        if (service is null || !service.IsActive)
            throw new InvalidOperationException(
                "Healthcare service is unavailable.");

        var canProvideService =
            await _staffServices.CanProvideServiceAsync(
                staff.Id, service.Id);

        if (!canProvideService)
            throw new InvalidOperationException(
                "Staff member cannot provide this service.");

        // 6. Calculate appointment duration.
        var end = request.Start.AddMinutes(
            service.DurationMinutes);

        var range = new TimeRange(request.Start, end);

        // 7. Validate branch working hours.
        var withinWorkingHours =
            await _workingHours.IsWithinWorkingHoursAsync(
                branch.Id, range.Start, range.End);

        if (!withinWorkingHours)
            throw new InvalidOperationException(
                "Appointment is outside branch working hours.");

        // 8. Validate staff availability.
        var isAvailable = await _availability.IsAvailableAsync(
            staff.Id, range.Start, range.End);

        if (!isAvailable)
            throw new InvalidOperationException(
                "Staff member is not available at this time.");

        // 9. Check appointment conflicts.
        var hasConflict = await _appointments.HasConflictAsync(
            staff.Id,
            patient.Id,
            range.Start,
            range.End);

        if (hasConflict)
            throw new InvalidOperationException(
                "Appointment conflicts with an existing booking.");

        // 10. Apply institution approval strategy.
        var strategy = _strategies.SingleOrDefault(
            s => s.Mode == organization.ApprovalMode);

        if (strategy is null)
            throw new InvalidOperationException(
                "No approval strategy is configured.");

        var initialStatus = strategy.DetermineInitialStatus();

        // 11. Create appointment.
        var appointment = new Appointment(
            patient.Id,
            staff.Id,
            branch.Id,
            service.Id,
            range.Start,
            range.End,
            service.Price,
            initialStatus);

        // 12. Persist through repository.
        await _appointments.AddAsync(appointment);

        return appointment;
    }
}
