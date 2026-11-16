using HospitalAppointmentSystem.Domain.Entities;
using HospitalAppointmentSystem.Domain.Enums;
using Xunit;

namespace HospitalAppointmentSystem.UnitTests.Domain;

public class StaffRegistrationRequestTests
{
    [Fact]
    public void NewApplication_ShouldStartAsPending()
    {
        var request = new StaffRegistrationRequest(
            "test-user-id",
            1,
            Profession.Doctor,
            "Cardiology");

        Assert.Equal(StaffRegistrationStatus.Pending, request.Status);
        Assert.Null(request.ReviewedAt);
        Assert.Null(request.ReviewedByUserId);
    }

    [Fact]
    public void Approve_ShouldChangeStatusAndRecordReviewer()
    {
        var request = new StaffRegistrationRequest(
            "test-user-id",
            1,
            Profession.Doctor,
            "Cardiology");

        request.Approve("admin-id");

        Assert.Equal(StaffRegistrationStatus.Approved, request.Status);
        Assert.Equal("admin-id", request.ReviewedByUserId);
        Assert.NotNull(request.ReviewedAt);
    }

    [Fact]
    public void Reject_ShouldChangeStatusAndRecordReviewer()
    {
        var request = new StaffRegistrationRequest(
            "test-user-id",
            1,
            Profession.Nurse,
            null);

        request.Reject("admin-id");

        Assert.Equal(StaffRegistrationStatus.Rejected, request.Status);
        Assert.Equal("admin-id", request.ReviewedByUserId);
        Assert.NotNull(request.ReviewedAt);
    }

    [Fact]
    public void ReviewedApplication_ShouldNotBeReviewedAgain()
    {
        var request = new StaffRegistrationRequest(
            "test-user-id",
            1,
            Profession.Doctor,
            "Neurology");

        request.Reject("admin-id");

        Assert.Throws<InvalidOperationException>(
            () => request.Approve("another-admin"));
    }

    [Fact]
    public void DoctorApplication_ShouldRequireSpecialty()
    {
        Assert.Throws<ArgumentException>(() =>
            new StaffRegistrationRequest(
                "test-user-id",
                1,
                Profession.Doctor,
                null));
    }
}
