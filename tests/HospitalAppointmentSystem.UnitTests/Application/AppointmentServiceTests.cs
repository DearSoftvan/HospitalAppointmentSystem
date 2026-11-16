using HospitalAppointmentSystem.Application.DTOs;
using HospitalAppointmentSystem.Application.Interfaces;
using HospitalAppointmentSystem.Application.Services;
using HospitalAppointmentSystem.Application.Strategies;
using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;

namespace HospitalAppointmentSystem.UnitTests.Application;

public class AppointmentServiceTests
{
    private static readonly DateTime AppointmentStart =
        new(2026, 11, 20, 10, 0, 0);

    private static CreateAppointmentRequest CreateRequest() =>
        new(1, 1, 1, 1, AppointmentStart);

    private static AppointmentService CreateService(FakeStore store)
    {
        IApprovalStrategy[] strategies =
        [
            new AutomaticApprovalStrategy(),
            new ManualApprovalStrategy()
        ];

        return new AppointmentService(
            store, store, store, store, store,
            store, store, store, store, strategies);
    }

    [Fact]
    public async Task CreateAsync_AutomaticMode_ShouldApprove()
    {
        var store = new FakeStore(ApprovalMode.Automatic);
        var service = CreateService(store);

        var appointment = await service.CreateAsync(CreateRequest());

        Assert.Equal(AppointmentStatus.Approved, appointment.Status);
        Assert.Equal(500m, appointment.Price);
        Assert.Equal(AppointmentStart.AddMinutes(30), appointment.End);
        Assert.True(store.WasSaved);
    }

    [Fact]
    public async Task CreateAsync_ManualMode_ShouldRemainPending()
    {
        var store = new FakeStore(ApprovalMode.Manual);
        var service = CreateService(store);

        var appointment = await service.CreateAsync(CreateRequest());

        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        Assert.True(store.WasSaved);
    }

    [Fact]
    public async Task CreateAsync_WhenConflictExists_ShouldReject()
    {
        var store = new FakeStore(ApprovalMode.Automatic)
        {
            HasConflict = true
        };

        var service = CreateService(store);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(CreateRequest()));

        Assert.False(store.WasSaved);
    }

    [Fact]
    public async Task CreateAsync_WhenPatientInactive_ShouldReject()
    {
        var store = new FakeStore(ApprovalMode.Automatic);
        store.Patient.Deactivate();

        var service = CreateService(store);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(CreateRequest()));

        Assert.False(store.WasSaved);
    }

    private sealed class FakeStore :
        IPatientRepository,
        IStaffRepository,
        IBranchRepository,
        IOrganizationRepository,
        IHealthcareServiceRepository,
        IStaffServiceRepository,
        IStaffAvailabilityRepository,
        IBranchWorkingHoursRepository,
        IAppointmentRepository
    {
        public Patient Patient { get; }
        public Staff Staff { get; }
        public Branch Branch { get; }
        public HealthcareOrganization Organization { get; }
        public HealthcareService Service { get; }

        public bool HasConflict { get; set; }
        public bool WasSaved { get; private set; }

        public FakeStore(ApprovalMode approvalMode)
        {
            Patient = new Patient("Test Patient");
            Branch = new Branch(1, "Main Branch", "Test Address");
            Staff = new Doctor(1, "Test Doctor", "Cardiology");
            Organization = new HealthcareOrganization(
                "Test Hospital", approvalMode);
            Service = new HealthcareService(
                "Examination", 30, 500m);

            // Simulate IDs assigned by a database.
            SetId(Patient, 1);
            SetId(Staff, 1);
            SetId(Branch, 1);
            SetId(Organization, 1);
            SetId(Service, 1);
        }

        private static void SetId<T>(T entity, int id)
            where T : class
        {
            var property = typeof(T).GetProperty("Id")
                ?? throw new InvalidOperationException(
                    "Entity ID property was not found.");

            property.SetValue(entity, id);
        }

        Task<Patient?> IPatientRepository.GetByIdAsync(int id) =>
            Task.FromResult<Patient?>(Patient);

        Task<Staff?> IStaffRepository.GetByIdAsync(int id) =>
            Task.FromResult<Staff?>(Staff);

        Task<Branch?> IBranchRepository.GetByIdAsync(int id) =>
            Task.FromResult<Branch?>(Branch);

        Task<HealthcareOrganization?>
            IOrganizationRepository.GetByIdAsync(int id) =>
            Task.FromResult<HealthcareOrganization?>(Organization);

        Task<HealthcareService?>
            IHealthcareServiceRepository.GetByIdAsync(int id) =>
            Task.FromResult<HealthcareService?>(Service);

        Task<bool> IStaffServiceRepository.CanProvideServiceAsync(
            int staffId, int healthcareServiceId) =>
            Task.FromResult(true);

        Task<bool> IStaffAvailabilityRepository.IsAvailableAsync(
            int staffId, DateTime start, DateTime end) =>
            Task.FromResult(true);

        Task<bool>
            IBranchWorkingHoursRepository.IsWithinWorkingHoursAsync(
                int branchId, DateTime start, DateTime end) =>
            Task.FromResult(true);

        Task<Appointment?> IAppointmentRepository.GetByIdAsync(
            int id) =>
            Task.FromResult<Appointment?>(null);

        Task<bool> IAppointmentRepository.HasConflictAsync(
            int staffId, int patientId,
            DateTime start, DateTime end) =>
            Task.FromResult(HasConflict);

        Task IAppointmentRepository.AddAsync(
            Appointment appointment)
        {
            WasSaved = true;
            return Task.CompletedTask;
        }
    }
}
