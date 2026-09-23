using Domain.Common;
using MediatR;

namespace Application.Common.Messaging;

/// <summary>
/// Adapter that lets the pure Domain project keep IDomainEvent framework-free
/// while Application handlers still use MediatR notifications.
/// </summary>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;

public abstract class DomainEventHandler<TDomainEvent>
    : INotificationHandler<DomainEventNotification<TDomainEvent>>
    where TDomainEvent : IDomainEvent
{
    public Task Handle(
        DomainEventNotification<TDomainEvent> notification,
        CancellationToken cancellationToken) =>
        HandleDomainEvent(notification.DomainEvent, cancellationToken);

    protected abstract Task HandleDomainEvent(
        TDomainEvent domainEvent,
        CancellationToken cancellationToken);
}
