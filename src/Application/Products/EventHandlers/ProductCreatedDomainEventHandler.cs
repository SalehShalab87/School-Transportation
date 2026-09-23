using Application.Common.Messaging;
using Application.Products.IntegrationEvents;
using Domain.Products.Events;

namespace Application.Products.EventHandlers;

public sealed class ProductCreatedDomainEventHandler(IOutboxWriter outboxWriter)
    : DomainEventHandler<ProductCreatedDomainEvent>
{
    protected override Task HandleDomainEvent(
        ProductCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        outboxWriter.Enqueue(new ProductCreatedIntegrationEvent(
            Guid.CreateVersion7(),
            domainEvent.ProductId,
            domainEvent.Name,
            domainEvent.OccurredOnUtc));

        return Task.CompletedTask;
    }
}
