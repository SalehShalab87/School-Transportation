using Domain.Common;

namespace Domain.Products.Events;

public sealed record ProductSoftDeletedDomainEvent(
    Guid ProductId,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;
