using EasyNetQ;
using EasyNetQ.Producer;
using System;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcConnection
    {
        public readonly IBus RabbitBus;

        private readonly ISerializer _serializer;

        public NetqRabbitRpcConnection(RpcConfiguration configuration, string hostname, string username, string password, int heartBeat = 10, int preFetch = 50, ushort timeout = 10,
            bool publisherConfirms = false, bool persistent = true, string product = null, string platform = null, string virtualHost = null,
            ISerializer serializer = null, RpcType rpcType = RpcType.Concrete)
        {
            _serializer = serializer;
            string connectionString = $"host={hostname};username={username};password={password};requestedHeartbeat={heartBeat};prefetchcount={heartBeat};" +
                $"persistentMessages={persistent};publisherConfirms={publisherConfirms}";
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

            RabbitBus = RabbitHutch.CreateBus(connectionString, registerer =>
            {
                registerer.Register(configuration);
                registerer.Register<IRpc, RpcRabbitWrapper>();
                registerer.Register<IMessageSerializationStrategy, ConcreteMessageSerializationStrategy>();
                registerer.Register(serializer);
            });
        }

        public object GetObject(byte[] obj, Type type)
        {
            return _serializer.Deserialize(obj, type);
        }
    }
}
