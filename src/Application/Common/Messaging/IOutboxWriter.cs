namespace Application.Common.Messaging;

public interface IOutboxWriter
{
    void Enqueue(IIntegrationEvent integrationEvent);
}
