using Application.Common.Messaging;

namespace Application.Products.IntegrationEvents;

public sealed record ProductCreatedIntegrationEvent(
    Guid EventId,
    Guid ProductId,
    string Name,
    DateTimeOffset OccurredOnUtc) : IIntegrationEvent;
