namespace Domain.Common;

/// <summary>
/// Marker for facts raised by domain aggregates. The Domain project intentionally
/// has no dependency on MediatR or any infrastructure framework.
/// </summary>
public interface IDomainEvent;
