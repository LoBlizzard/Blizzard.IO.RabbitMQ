using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;
using EasyNetQ.Producer;
using System;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcConnection : INetqRpcRabbitConnection<Func<Type, object>>
    {
        public IBus Bus { get; }
        public RpcMessageType RpcMessageType { get; }
        private readonly ILogger<NetqRabbitRpcConnection> _logger;

        public NetqRabbitRpcConnection(RpcConfiguration configuration, string hostname, string username, string password, ILoggerFactory loggerFactory, 
            int heartBeat = 10, int preFetch = 50, ushort timeout = 10,bool publisherConfirms = false, bool persistent = true, string product = null, 
            string platform = null, string virtualHost = null,ISerializer serializer = null, RpcMessageType rpcMessageType = RpcMessageType.Concrete)
        {
            _logger = loggerFactory.CreateLogger<NetqRabbitRpcConnection>();
            RpcMessageType = rpcMessageType;

            string connectionString = GetConnectionString(hostname, username, password, heartBeat, preFetch, timeout, publisherConfirms, persistent,
                product, platform, virtualHost);

            Bus = RabbitHutch.CreateBus(connectionString, registerer =>
            {
                switch (rpcMessageType)
                {
                    case RpcMessageType.Abstract:
                        registerer.Register<IMessageSerializationStrategy, AbstractMessageSerializationStrategy>();
                        break;
                    default:
                        registerer.Register<IMessageSerializationStrategy, ConcreteMessageSerializationStrategy>();
                        break;
                }

                registerer.Register(configuration);
                registerer.Register<IRpc, RpcRabbitWrapper>();
                registerer.Register(serializer);
            });

            _logger.LogInformation($"Created new RpcRabbit connection to host: {hostname}, of type: {rpcMessageType}");
        }

        private string GetConnectionString(string hostname, string username, string password, int heartBeat, int preFetch, ushort timeout,
            bool publisherConfirms, bool persistent, string product,string platform, string virtualHost)
        {
            string connectionString = $"host={hostname};username={username};password={password};requestedHeartbeat={heartBeat};prefetchcount={preFetch};" +
                $"persistentMessages={persistent};publisherConfirms={publisherConfirms};timeout={timeout};";
            if (product != null)
            {
                connectionString += $"product={product}";
            }
            if (platform != null)
            {
                connectionString += $"platform={platform}";
            }
            if (virtualHost != null)
            {
                connectionString += $"virtualHost={virtualHost}";
            }

            return connectionString;
        }
    }
}
