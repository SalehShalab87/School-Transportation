using Domain.Common;
using Domain.TransportationRequests;

namespace Domain.UnitTests.TransportationRequests;

public class TransportRequestTests
{
    [Fact]
    public void Siblings_keep_independent_transport_directions_and_locations()
    {
        var request = NewRequest();
        var first = request.AddStudent(Guid.NewGuid(), "First", "1", "F", true, false);
        var second = request.AddStudent(Guid.NewGuid(), "Second", "10", "M", false, true);

        first.SetMorningPickup(31.9m, 35.9m, "guardian-link");
        second.SetAfternoonDropOff(31.8m, 35.8m, "staff");

        Assert.Null(first.AfternoonDropOff);
        Assert.Null(second.MorningPickup);
        Assert.NotEqual(first.Grade, second.Grade);
        Assert.False(request.IsReadyForPlanning);

        first.VerifyMorningPickup();
        second.VerifyAfternoonDropOff();
        Assert.True(request.IsReadyForPlanning);
    }

    [Fact]
    public void Missing_or_unverified_pin_keeps_request_out_of_planning()
    {
        var request = NewRequest();
        var student = request.AddStudent(Guid.NewGuid(), "Student", "3", "F", true, true);

        student.SetMorningPickup(31.9m, 35.9m, "guardian-link");
        student.VerifyMorningPickup();
        Assert.False(request.IsReadyForPlanning);

        student.SetAfternoonDropOff(31.8m, 35.8m, "guardian-link");
        Assert.False(request.IsReadyForPlanning);
        student.VerifyAfternoonDropOff();
        Assert.True(request.IsReadyForPlanning);
    }

    [Fact]
    public void Corrected_pin_requires_fresh_verification()
    {
        var student = NewRequest().AddStudent(Guid.NewGuid(), "Student", "3", "F", true, false);
        student.SetMorningPickup(31.9m, 35.9m, "guardian-link");
        student.VerifyMorningPickup();
        Assert.True(student.IsReadyForPlanning);

        student.SetMorningPickup(32.0m, 35.8m, "guardian-correction");
        Assert.False(student.IsReadyForPlanning);
        Assert.Equal("guardian-correction", student.MorningPickup!.Source);
    }

    [Theory]
    [InlineData(91, 35)]
    [InlineData(31, 181)]
    public void Invalid_coordinates_are_rejected(int latitude, int longitude)
    {
        var student = NewRequest().AddStudent(Guid.NewGuid(), "Student", "3", "F", true, false);
        Assert.Throws<DomainException>(() => student.SetMorningPickup(latitude, longitude, "staff"));
    }

    [Fact]
    public void Request_without_direction_is_rejected()
    {
        var request = NewRequest();
        Assert.Throws<DomainException>(() =>
            request.AddStudent(Guid.NewGuid(), "Student", "3", "F", false, false));
    }

    [Fact]
    public void Request_cannot_repeat_student_id()
    {
        var request = NewRequest();
        var id = Guid.NewGuid();
        request.AddStudent(id, "Student", "3", "F", true, false);
        Assert.Throws<DomainException>(() => request.AddStudent(id, "Student", "3", "F", true, false));
    }

    private static TransportRequest NewRequest() => new(Guid.NewGuid(), "Guardian", "+962700000000");
}
