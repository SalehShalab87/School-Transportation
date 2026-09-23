using System.Text.Json;
using Application.Common.Messaging;
using Infrastructure.Persistence;

namespace Infrastructure.Messaging.Outbox;

public sealed class OutboxWriter(ApplicationDbContext dbContext) : IOutboxWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public void Enqueue(IIntegrationEvent integrationEvent)
    {
        var type = integrationEvent.GetType().FullName
            ?? throw new InvalidOperationException("Integration event type has no full name.");

        var payload = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType(), SerializerOptions);

        dbContext.OutboxMessages.Add(new OutboxMessage(
            integrationEvent.EventId,
            integrationEvent.OccurredOnUtc,
            type,
            payload));
    }
}
