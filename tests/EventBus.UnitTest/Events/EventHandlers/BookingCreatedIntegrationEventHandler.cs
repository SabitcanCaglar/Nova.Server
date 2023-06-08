using EventBus.Base.Abstraction;
using EventBus.UnitTest.Events.Events;

namespace EventBus.UnitTest.Events.EventHandlers;

public class BookingCreatedIntegrationEventHandler:IIntegrationEventHandler<BookingCreatedIntegrationEvent>
{
    public Task Handle(BookingCreatedIntegrationEvent @event)
    {
        return Task.CompletedTask;
    }
}