using MediatR;

namespace Application.Common.Messaging;

/// <summary>
/// Durable/public application event. Persist it in the outbox before asynchronous delivery.
/// Consumers must be idempotent because outbox delivery is at-least-once.
/// </summary>
public interface IIntegrationEvent : INotification
{
    Guid EventId { get; }
    DateTimeOffset OccurredOnUtc { get; }
}
