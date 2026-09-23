namespace Infrastructure.Messaging.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(
        Guid id,
        DateTimeOffset occurredOnUtc,
        string type,
        string payload)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Payload = payload;
    }

    public Guid Id { get; private set; }
    public DateTimeOffset OccurredOnUtc { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset? ProcessedOnUtc { get; private set; }
    public int AttemptCount { get; private set; }
    public string? LastError { get; private set; }

    public void MarkProcessed(DateTimeOffset now)
    {
        ProcessedOnUtc = now;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        AttemptCount++;
        LastError = error.Length <= 4000 ? error : error[..4000];
    }
}
