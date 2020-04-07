using Blizzard.IO.Core.Rpc;
using Microsoft.Extensions.Logging;
using Blizzard.IO.RabbitMQ.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using System;

namespace Blizzard.IO.RabbitMQ.Builders.Rpc
{
    public class NetqRabbitRpcBuilder : BaseNetqRabbitRpcBuilder
    {
        public NetqRabbitRpcBuilder(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public NetqRabbitRpcBuilder AddRpcMessageType(RpcMessageType rpcMessageType)
        {
            RpcMessageType = rpcMessageType;
            return this;
        }

        public NetqRabbitRpcBuilder AddRabbitConnectionProperties(
            ushort timeout = 10,
            string product = null,
            string platform = null,
            string virtualHost = null,
            int requestHeartbeat = 10,
            int prefetchCount = 50,
            bool publisherConfirms = false,
            bool persistentMessages = true)
        {
            Timeout = timeout;
            PrefetchCount = prefetchCount;
            Product = product;
            Platform = platform;
            PublisherConfirms = publisherConfirms;
            RequestHeartbeat = requestHeartbeat;
            VirtualHost = virtualHost;
            PersistentMessages = persistentMessages;
            return this;
        }

        public NetqRabbitRpcBuilder AddCredentials(string hostname, string username, string password)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
            return this;
        }

        public NetqRabbitRpcBuilder AddSerializer(ISerializer serializer)
        {
            Serializer = serializer;
            return this;
        }

        public NetqRabbitRpcBuilder AddRpcConfiguration(RpcConfiguration rpcConfiguration)
        {
            RpcConfiguration = rpcConfiguration;
            return this;
        }

        public IRpcClient BuildClient()
        {
            INetqRabbitRpcConnection connection = InitConnection();

            return new NetqRabbitRpcClient(connection, LoggerFactory);
        }

        public IRpcServer BuildServer()
        {
            INetqRabbitRpcConnection connection = InitConnection();

            return new NetqRabbitRpcServer(connection, LoggerFactory);
        }
    }
}
