using Blizzard.IO.Core.Rpc;
using System.Collections.Generic;
using Blizzard.IO.RabbitMQ.Rpc;
using System;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Microsoft.Extensions.Logging;
using Blizzard.IO.Serialization.Rpc;

namespace Blizzard.IO.RabbitMQ.Builders.Rpc
{
    public class BaseNetqRabbitRpcBuilder
    {
        private static Dictionary<RpcConnectionKey, INetqRabbitRpcConnection<Func<Type, object>>> netqRpcConnections = new Dictionary<RpcConnectionKey, INetqRabbitRpcConnection<Func<Type, object>>>();

        protected ISerializer Serializer = new JsonSerializer();
        protected string Hostname = "localhost";
        protected string Password = "guest";
        protected string Username = "guest";
        protected RpcConfiguration RpcConfiguration = new RpcConfiguration
        {
            ExchangeNameProvider = type => "RPC_EXCHANGE",
            RoutingKeyProvider = type => type.ToString()
        };
        protected RpcMessageType RpcMessageType = RpcMessageType.Concrete;
        protected ushort Timeout = 10;
        protected string Product = null;
        protected string Platform = null;
        protected string VirtualHost = null;
        protected int RequestHeartbeat = 10;
        protected int PrefetchCount = 50;
        protected bool PublisherConfirms = false;
        protected bool PersistentMessages = true;
        protected readonly ILoggerFactory LoggerFactory;

        public BaseNetqRabbitRpcBuilder(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        protected INetqRabbitRpcConnection<Func<Type, object>> InitConnection()
        {
            RpcConnectionKey busKey = new RpcConnectionKey(Hostname, Username, Password, RpcMessageType, Serializer.GetType());
            if (netqRpcConnections.ContainsKey(busKey))
            {
                return netqRpcConnections[busKey];
            }

            INetqRabbitRpcConnection<Func<Type,object>> connection = new NetqRabbitRpcConnection(RpcConfiguration, Hostname, Username, Password,LoggerFactory,
               Serializer, RequestHeartbeat, (ushort)PrefetchCount,Timeout,PublisherConfirms, PersistentMessages, Product, Platform, VirtualHost);
            netqRpcConnections.Add(busKey, connection);

            return connection;
        }
    }
}
