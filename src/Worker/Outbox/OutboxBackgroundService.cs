using Application.Common.Messaging;

namespace Worker.Outbox;

public sealed class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<OutboxBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pollInterval = TimeSpan.FromSeconds(
            Math.Max(1, configuration.GetValue("Outbox:PollIntervalSeconds", 5)));
        var batchSize = Math.Clamp(configuration.GetValue("Outbox:BatchSize", 50), 1, 500);

        logger.LogInformation(
            "Outbox worker started. Poll interval: {PollInterval}; Batch size: {BatchSize}",
            pollInterval,
            batchSize);

        using var timer = new PeriodicTimer(pollInterval);

        do
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                await processor.ProcessPendingAsync(batchSize, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled outbox processing error");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
