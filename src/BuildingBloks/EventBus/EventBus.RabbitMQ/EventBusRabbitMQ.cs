using EventBus.Base;
using EventBus.Base.Events;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ :BaseEventBus
{
    public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
    {
    }

    public override void Publish(IntegrationEvent @event)
    {
        throw new NotImplementedException();
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!EventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            
        }
    }

    public override void UnSubscribe<T, TH>()
    {
        throw new NotImplementedException();
    }
}