using System.Net.Sockets;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ :BaseEventBus
{
    private RabbitMQPersistentConnection _persistentConnection;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IModel _consumerChannel;

    public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(serviceProvider, config)
    {
        if (config.Connection is not null)
        {
            var connectionJson = JsonConvert.SerializeObject(_eventBusBaseConfig.Connection,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            _connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connectionJson);
        }
        else
            _connectionFactory = new ConnectionFactory();
        
        _persistentConnection = new RabbitMQPersistentConnection(_connectionFactory,config.ConnectionRetryCount);
        
        _consumerChannel = CreateConsumerChannel();
        
        EventBusSubscriptionManager.OnEventRemoved += EventBusSubscriptionManagerOnOnEventRemoved;
    }

    private void EventBusSubscriptionManagerOnOnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);

        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }
        _consumerChannel.QueueBind(queue:eventName,
            exchange: _eventBusBaseConfig.DefaultTopicName,
            routingKey:eventName);

        if (EventBusSubscriptionManager.IsEmpty)
        {
            _consumerChannel.Close();
        }
    }

    public override void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }
        
        var policy = Policy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(_eventBusBaseConfig.ConnectionRetryCount,retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),(ex,time) =>
            {
                // log error 
            });

        var eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);
        
        _consumerChannel.ExchangeDeclare(exchange:_eventBusBaseConfig.DefaultTopicName, type: "direct");

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = _consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; // persistent

            _consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exists while publishing
            durable:true,
            exclusive:false,
            autoDelete:false,
            arguments:null);
            
            _consumerChannel.BasicPublish(
                exchange:_eventBusBaseConfig.DefaultTopicName,
                routingKey:eventName,
                mandatory:true,
                basicProperties: properties,
                body: body);
        });
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!EventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exists while consuming
                durable:true,
                exclusive:false,
                autoDelete: false,
                arguments:null);
            
            _consumerChannel.QueueBind(queue:GetSubName(eventName),
                exchange:_eventBusBaseConfig.DefaultTopicName,
                routingKey: eventName);
        }
        
        EventBusSubscriptionManager.AddSubscription<T,TH>();
        StartBasicConsume(eventName);
    }

    public override void UnSubscribe<T, TH>()
    {
        EventBusSubscriptionManager.RemoveSubscription<T,TH>();
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var channel = _persistentConnection.CreateModel();
        
        channel.ExchangeDeclare(exchange:_eventBusBaseConfig.DefaultTopicName,type: "direct");

        return channel;
    }

    private void StartBasicConsume(string eventName)
    {
        if (_consumerChannel is not null)
        {
            var consumer = new /*Async*/ EventingBasicConsumer(_consumerChannel);
            
            consumer.Received += ConsumerOnReceived;
            
            _consumerChannel.BasicConsume(
                queue:GetSubName(eventName),
                autoAck:false,
                consumer: consumer);
        }
    }

    private async void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        eventName = ProcessEventName(eventName);
        var message = Encoding.UTF8.GetString(e.Body.Span);

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception exception)
        {
            // loging
            Console.WriteLine(exception);
            throw;
        }
        
        _consumerChannel.BasicAck(e.DeliveryTag,multiple:false);
    }
}