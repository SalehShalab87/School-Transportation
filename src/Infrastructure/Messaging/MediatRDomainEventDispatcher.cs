using Application.Common.Messaging;
using Domain.Common;
using MediatR;

namespace Infrastructure.Messaging;

public sealed class MediatRDomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        var notification = Activator.CreateInstance(notificationType, domainEvent)
            ?? throw new InvalidOperationException($"Could not create notification for {domainEvent.GetType().Name}.");

        return publisher.Publish(notification, cancellationToken);
    }
}
