using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events;

public abstract class BaseEventBus : IEventBus
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

    public virtual string ProcessEventName(string eventName)
    {
        if (_eventBusConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(_eventBusConfig.EventNamePrefix.ToArray());
        if (_eventBusConfig.DeleteEventSuffix)
            eventName = eventName.TrimStart(_eventBusConfig.EventNameSuffix.ToArray());

        return eventName;
    }

    public virtual string GetSubName(string eventName)
    {
        return $"{_eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
    }

    public virtual void Dispose()
    {
        _eventBusConfig = null;
    }

    public async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);

        var processed = false;

        if (!EventBusSubscriptionManager.HasSubscriptionsForEvent(eventName)) return processed;

        var subscriptions = EventBusSubscriptionManager.GetHandlersForEvent(eventName);

        using (var scope = ServiceProvider.CreateScope())
        {
            foreach (var subscription in subscriptions)
            {
                var handler = ServiceProvider.GetService(subscription.HandlerType);
                if (handler is null)
                    continue;

                var eventType = EventBusSubscriptionManager.GetEventTypeByName($"{_eventBusConfig.EventNamePrefix}{eventName}{_eventBusConfig.EventNameSuffix}");
                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                var concoreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                await (Task)concoreteType.GetMethod("Handle")?.Invoke(handler, new object[] { integrationEvent });
            }
        }

        processed = true;

        return processed;
    }

    public abstract void Publish(IntegrationEvent @event);

    public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

    public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
}