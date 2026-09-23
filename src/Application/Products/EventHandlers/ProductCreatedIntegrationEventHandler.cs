using Application.Products.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Products.EventHandlers;

/// <summary>
/// Demonstrates an asynchronous integration-event consumer. In the transportation
/// solution, handlers like this will react to PlanApproved, StudentAbsentForSession,
/// PaymentReceived, etc. Keep consumers idempotent.
/// </summary>
public sealed class ProductCreatedIntegrationEventHandler(
    ILogger<ProductCreatedIntegrationEventHandler> logger)
    : INotificationHandler<ProductCreatedIntegrationEvent>
{
    public Task Handle(
        ProductCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Integration event delivered: product {ProductId} ({ProductName}) created",
            notification.ProductId,
            notification.Name);

        return Task.CompletedTask;
    }
}
