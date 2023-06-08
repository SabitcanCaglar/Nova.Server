using EventBus.Base.Events;

namespace EventBus.UnitTest.Events.Events;

public class BookingCreatedIntegrationEvent : IntegrationEvent
{
    public BookingCreatedIntegrationEvent(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}