using Microsoft.Extensions.DependencyInjection;

namespace EventBus.UnitTest;

public class EventBusTests
{
    private ServiceCollection _serviceCollection;

    public EventBusTests()
    {
        _serviceCollection = new ServiceCollection();
    }

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}