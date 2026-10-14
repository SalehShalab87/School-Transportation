using Domain.Common;

namespace Domain.TransportationRequests;

/// <summary>
/// Captures transportation demand only; it does not promise route feasibility or a seat.
/// </summary>
public sealed class TransportRequest : AggregateRoot
{
    private readonly List<StudentTransportDemand> _students = [];

    private TransportRequest() { } // EF Core
    public TransportRequest(Guid id, string guardianName, string guardianPhone) : base(id)
    {
        if (id == Guid.Empty) throw new DomainException("A request ID is required.");
        UpdateGuardian(guardianName, guardianPhone);
    }

    public string GuardianName { get; private set; } = string.Empty;
    public string GuardianPhone { get; private set; } = string.Empty;
    public IReadOnlyCollection<StudentTransportDemand> Students => _students.AsReadOnly();

    public void UpdateGuardian(string name, string phone)
    {
        GuardianName = Required(name, "Guardian name");
        GuardianPhone = Required(phone, "Guardian phone");
    }

    public StudentTransportDemand AddStudent(
        Guid studentId, string name, string grade, string gender,
        bool morningRequired, bool afternoonRequired)
    {
        if (studentId == Guid.Empty || _students.Any(s => s.Id == studentId))
            throw new DomainException("Student ID must be unique and nonempty within a request.");
        if (!morningRequired && !afternoonRequired)
            throw new DomainException("At least one transportation direction is required.");

        var student = new StudentTransportDemand(studentId, name, grade, gender, morningRequired, afternoonRequired);
        _students.Add(student);
        return student;
    }

    public bool IsReadyForPlanning =>
        _students.Count > 0 && _students.All(s => s.IsReadyForPlanning);

    internal static string Required(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException(field + " is required.");
        return value.Trim();
    }
}

public sealed class StudentTransportDemand : Entity
{
    private StudentTransportDemand() { } // EF Core

    internal StudentTransportDemand(Guid id, string name, string grade, string gender,
        bool morningRequired, bool afternoonRequired) : base(id)
    {
        Name = TransportRequest.Required(name, "Student name");
        Grade = TransportRequest.Required(grade, "Grade");
        Gender = TransportRequest.Required(gender, "Gender");
        MorningRequired = morningRequired;
        AfternoonRequired = afternoonRequired;
    }

    public string Name { get; private set; } = string.Empty;
    public string Grade { get; private set; } = string.Empty;
    public string Gender { get; private set; } = string.Empty;
    public bool MorningRequired { get; private set; }
    public bool AfternoonRequired { get; private set; }
    public TransportLocation? MorningPickup { get; private set; }
    public TransportLocation? AfternoonDropOff { get; private set; }

    public bool IsReadyForPlanning =>
        (!MorningRequired || MorningPickup?.Verification == LocationVerification.Verified) &&
        (!AfternoonRequired || AfternoonDropOff?.Verification == LocationVerification.Verified);

    public void SetMorningPickup(decimal latitude, decimal longitude, string source)
    {
        if (!MorningRequired) throw new DomainException("Morning transport is not requested.");
        MorningPickup = TransportLocation.Supplied(latitude, longitude, source);
    }

    public void SetAfternoonDropOff(decimal latitude, decimal longitude, string source)
    {
        if (!AfternoonRequired) throw new DomainException("Afternoon transport is not requested.");
        AfternoonDropOff = TransportLocation.Supplied(latitude, longitude, source);
    }

    public void VerifyMorningPickup()
    {
        if (MorningPickup is null) throw new DomainException("Morning pickup is missing.");
        MorningPickup = MorningPickup with { Verification = LocationVerification.Verified };
    }

    public void VerifyAfternoonDropOff()
    {
        if (AfternoonDropOff is null) throw new DomainException("Afternoon drop-off is missing.");
        AfternoonDropOff = AfternoonDropOff with { Verification = LocationVerification.Verified };
    }
}

public enum LocationVerification { Supplied, Verified }

/// <summary>
/// Coordinates are user-supplied, untrusted until explicitly verified.
/// No school serviceability is inferred from valid coordinate ranges.
/// </summary>
public sealed record TransportLocation
{
    private TransportLocation() { } // EF Core
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public string Source { get; init; } = string.Empty;
    public LocationVerification Verification { get; init; }

    public static TransportLocation Supplied(decimal latitude, decimal longitude, string source)
    {
        if (latitude < -90m || latitude > 90m || longitude < -180m || longitude > 180m)
            throw new DomainException("Map coordinates are outside valid geographic bounds.");
        return new TransportLocation
        {
            Latitude = latitude,
            Longitude = longitude,
            Source = TransportRequest.Required(source, "Location source"),
            Verification = LocationVerification.Supplied
        };
    }
}
