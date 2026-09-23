using Domain.Common;

namespace Domain.Products.Events;

public sealed record ProductCreatedDomainEvent(
    Guid ProductId,
    string Name,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
