using System.Text.Json;
using Application.Common.Abstractions;
using Application.Common.Messaging;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Outbox;

public sealed class OutboxProcessor(
    ApplicationDbContext dbContext,
    IPublisher publisher,
    IClock clock,
    ILogger<OutboxProcessor> logger) : IOutboxProcessor
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<int> ProcessPendingAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        var messages = await dbContext.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var eventType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(assembly => assembly.GetType(message.Type, throwOnError: false))
                    .FirstOrDefault(type => type is not null)
                    ?? throw new InvalidOperationException($"Unknown outbox event type: {message.Type}");

                var integrationEvent = JsonSerializer.Deserialize(message.Payload, eventType, SerializerOptions)
                    as IIntegrationEvent
                    ?? throw new InvalidOperationException($"Could not deserialize outbox event {message.Id}.");

                await publisher.Publish(integrationEvent, cancellationToken);
                message.MarkProcessed(clock.UtcNow);
            }
            catch (Exception exception)
            {
                message.MarkFailed(exception.ToString());
                logger.LogError(exception, "Failed to process outbox message {OutboxMessageId}", message.Id);
            }
        }

        if (messages.Count != 0)
            await dbContext.SaveChangesAsync(cancellationToken);

        return messages.Count;
    }
}
