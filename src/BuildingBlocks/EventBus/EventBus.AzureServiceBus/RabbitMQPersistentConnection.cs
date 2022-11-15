using RabbitMQ.Client;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Exceptions;

namespace EventBus.AzureServiceBus
{
    public class RabbitMQPersistentConnection : IDisposable
    {

        private IConnectionFactory connectionFactory;
        private IConnection connection;
        private int retryCount;
        private object lock_Object = new object();
        private bool _disposed;

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory,int retryCount=5)
        {
            this.connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }

        public bool IsConnected => connection != null && connection.IsOpen;

        public IModel CreateModel()
        {
            return connection.CreateModel();
        }
        public void Dispose()
        {
            _disposed = true;
            connection.Dispose();
        }
        public bool TryConnect()
        {
            lock(lock_Object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount,retryAttemp =>TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),(ex,time)=>
                    {

                    });
                policy.Execute(() =>
                    connection = connectionFactory.CreateConnection()
                );
                if(IsConnected)
                {
                    connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    connection.CallbackException += Connection_CallbackException;

                    return true;
                }
                return false;
            }
                
        }
        private void Connection_ConnectionBlocked(object sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
        private void Connection_CallbackException(object sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
        private void Connection_ConnectionShutdown(object sender,ShutdownEventArgs e)
        {
            if (_disposed) return;
            TryConnect();
        }
    }
}
