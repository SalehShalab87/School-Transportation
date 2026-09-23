using Application.Common.Abstractions.Persistence;
using Application.Common.Messaging;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class UnitOfWork(
    ApplicationDbContext dbContext,
    IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch in-memory domain facts before the single EF SaveChanges call.
        // Handlers may add outbox rows or other tracked changes, so everything is
        // committed atomically by this SaveChanges transaction.
        while (true)
        {
            var aggregates = dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count != 0)
                .ToArray();

            if (aggregates.Length == 0)
                break;

            var events = aggregates.SelectMany(entity => entity.DomainEvents).ToArray();
            foreach (var aggregate in aggregates)
                aggregate.ClearDomainEvents();

            foreach (var domainEvent in events)
                await domainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);
        }

        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
