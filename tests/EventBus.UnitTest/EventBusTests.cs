using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.UnitTest;

public class EventBusTests
{
    private readonly ServiceCollection _serviceCollection;

    public EventBusTests()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddLogging(config => config.AddConsole());
    }

    [SetUp]
    public void Setup()
    {
        _serviceCollection.AddSingleton<IEventBus>(sp =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "EventBus.UnitTest.TopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent"
                //Connection = new ConnectionFactory()
                //{
                //HostName = "localhost",
                //Port = 5672,
                //UserName = "guest",
                //Password = "guest"
                //}
            };
            return EventBusFactory.Create(config,sp);
        });
    }

    [Test]
    public void subscribe_event_on_rabbitmq_test()
    {
        var sp = _serviceCollection.BuildServiceProvider();
        
        var eventBus = sp.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<BookingCreatedIntegrationEvent,BookingCreatedIntegrationEventHandler>();
       
        Assert.Pass();
    }
}