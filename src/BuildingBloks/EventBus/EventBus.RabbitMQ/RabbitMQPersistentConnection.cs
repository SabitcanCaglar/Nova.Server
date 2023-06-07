using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ;

public class RabbitMQPersistentConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    public readonly int _retryCount;
    private          IConnection        _connection;
    private          object             _lockObject = new object();
    public bool IsConnected => _connection is not null && _connection.IsOpen;
    private bool _disposed;


    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
    {
        _connectionFactory = connectionFactory;
        _retryCount = retryCount;
    }
    
    public IModel CreateModel()
    {
        return _connection.CreateModel();
    }
    public void Dispose()
    {
        _disposed = true;
        _connection.Dispose();
    }

    public bool TryConnect()
    {
        lock (_lockObject)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount,retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)),(ex,time) =>
                {
                });

            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();
            });

            if (IsConnected)
            {
                _connection.ConnectionShutdown += ConnectionOnConnectionShutdown;
                _connection.CallbackException += ConnectionOnCallbackException;
                _connection.ConnectionBlocked += ConnectionOnConnectionBlocked;
                // LOG success
                return true;
            }

            return false;
        }
    }

    private void ConnectionOnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        ShutDownOperation();
    }

    private void ConnectionOnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        ShutDownOperation();
    }

    private void ConnectionOnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        ShutDownOperation();
    }

    private void ShutDownOperation()
    {
        if(_disposed)
            return;
        
        // log connection shutdown.

        TryConnect();
    }
}