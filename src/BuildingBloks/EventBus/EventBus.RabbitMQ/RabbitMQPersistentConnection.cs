namespace EventBus.RabbitMQ;

public class RabbitMQPersistentConnection : IDisposable
{
    public bool IsConnection { get; }
    
    public void Dispose()
    {
    }
}