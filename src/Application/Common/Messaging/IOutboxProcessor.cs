namespace Application.Common.Messaging;

public interface IOutboxProcessor
{
    Task<int> ProcessPendingAsync(int batchSize, CancellationToken cancellationToken = default);
}
