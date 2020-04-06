using Blizzard.IO.Core.Rpc;
using System.Collections.Generic;
using Blizzard.IO.RabbitMQ.Rpc;
using System;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ.Builders.Rpc
{
    public class BaseNetqRabbitRpcBuilder
    {
        private static Dictionary<RpcConnectionKey, INetqRabbitRpcConnection<Func<Type, object>>> netqRpcConnections = new Dictionary<RpcConnectionKey, INetqRabbitRpcConnection<Func<Type, object>>>();

        private readonly ILoggerFactory _loggerFactory;

        public BaseNetqRabbitRpcBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected INetqRabbitRpcConnection<Func<Type, object>> InitConnection(
            string host,
            string password,
            string username,
            RpcConfiguration configuration,
            ISerializer serializer,
            RpcMessageType rpcMessageType = RpcMessageType.Concrete,
            ushort timeout = 10,
            string product = null,
            string platform = null,
            string virtualHost = null,
            int requestHeartbeat = 10,
            int prefetchCount = 50,
            bool publisherConfirms = false,
            bool persistentMessages = true)
        {
            RpcConnectionKey busKey = new RpcConnectionKey(host, username, password, rpcMessageType);
            if (netqRpcConnections.ContainsKey(busKey))
            {
                return netqRpcConnections[busKey];
            }

            INetqRabbitRpcConnection<Func<Type,object>> connection = new NetqRabbitRpcConnection(configuration, host, username, password,_loggerFactory, serializer, requestHeartbeat, (ushort)prefetchCount, timeout,
                publisherConfirms, persistentMessages, product, platform, virtualHost);
            netqRpcConnections.Add(busKey, connection);

            return connection;
        }
    }
}
