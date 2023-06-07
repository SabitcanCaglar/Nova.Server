using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events;

public abstract class BaseEventBus : IEventBus
{
    public readonly IServiceProvider ServiceProvider;
    public readonly IEventBusSubscriptionManager EventBusSubscriptionManager;

    public EventBusConfig _eventBusBaseConfig;

    protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig config)
    {
        ServiceProvider             = serviceProvider;
        EventBusSubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        _eventBusBaseConfig         = config;
    }

    public virtual string ProcessEventName(string eventName)
    {
        if (_eventBusBaseConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(_eventBusBaseConfig.EventNamePrefix.ToArray());
        if (_eventBusBaseConfig.DeleteEventSuffix)
            eventName = eventName.TrimStart(_eventBusBaseConfig.EventNameSuffix.ToArray());

        return eventName;
    }

    public virtual string GetSubName(string eventName)
    {
        return $"{_eventBusBaseConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
    }

    public virtual void Dispose()
    {
        _eventBusBaseConfig = null;
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

                var eventType = EventBusSubscriptionManager.GetEventTypeByName($"{_eventBusBaseConfig.EventNamePrefix}{eventName}{_eventBusBaseConfig.EventNameSuffix}");
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