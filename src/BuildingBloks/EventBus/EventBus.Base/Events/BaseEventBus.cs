using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;

namespace EventBus.Base.Events;

public abstract class BaseEventBus :IEventBus
{
    public readonly IServiceProvider ServiceProvider;
    public readonly IEventBusSubscriptionManager EventBusSubscriptionManager;

    private EventBusConfig _eventBusConfig;

    protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig)
    {
        ServiceProvider             = serviceProvider;
        EventBusSubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        _eventBusConfig             = eventBusConfig;
    }

    private string ProcessEventName(string eventName)
    {
        if (_eventBusConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(_eventBusConfig.EventNamePrefix.ToArray());
        if (_eventBusConfig.DeleteEventSuffix)
            eventName = eventName.TrimStart(_eventBusConfig.EventNameSuffix.ToArray());

        return eventName;
    }
    
    public Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);
        
        var processed = false;

        if (EventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptions = EventBusSubscriptionManager.GetHandlersForEvent(eventName);

            return true;
        }
        return false;
    }

    public void Publish(IntegrationEvent @event)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }

    public void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        throw new NotImplementedException();
    }
}