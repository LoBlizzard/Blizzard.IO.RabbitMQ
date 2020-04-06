using Blizzard.IO.Core.Rpc;
using Microsoft.Extensions.Logging;
using Blizzard.IO.RabbitMQ.Rpc;
using Blizzard.IO.Serialization.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using System;

namespace Blizzard.IO.RabbitMQ.Builders.Rpc
{
    public class NetqRabbitRpcBuilder : BaseNetqRabbitRpcBuilder
    {
        private ISerializer _serializer = new JsonSerializer();
        private string _hostname = "localhost";
        private string _password = "guest";
        private string _username = "guest";
        private RpcConfiguration _rpcConfiguration = new RpcConfiguration
        {
            ExchangeNameProvider = type => "RPC_EXCHANGE",
            RoutingKeyProvider = type => type.ToString()
        };
        private RpcMessageType _rpcMessageType = RpcMessageType.Concrete;
        private ushort _timeout = 10;
        private string _product = null;
        private string _platform = null;
        private string _virtualHost = null;
        private int _requestHeartbeat = 10;
        private int _prefetchCount = 50;
        private bool _publisherConfirms = false;
        private bool _persistentMessages = true;


        public NetqRabbitRpcBuilder(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public NetqRabbitRpcBuilder AddRpcMessageType(RpcMessageType rpcMessageType)
        {
            _rpcMessageType = rpcMessageType;
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
            _timeout = timeout;
            _prefetchCount = prefetchCount;
            _product = product;
            _platform = platform;
            _publisherConfirms = publisherConfirms;
            _requestHeartbeat = requestHeartbeat;
            _virtualHost = virtualHost;
            _persistentMessages = persistentMessages;
            return this;
        }

        public NetqRabbitRpcBuilder AddCredentials(string hostname, string username, string password)
        {
            _hostname = hostname;
            _username = username;
            _password = password;
            return this;
        }

        public NetqRabbitRpcBuilder AddSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public NetqRabbitRpcBuilder AddRpcConfiguration(RpcConfiguration rpcConfiguration)
        {
            _rpcConfiguration = rpcConfiguration;
            return this;
        }

        public IRpcClient BuildClient()
        {
            INetqRabbitRpcConnection<Func<Type, object>> connection = InitConnection(_hostname, _password, _username, _rpcConfiguration, _serializer,
                _rpcMessageType, _timeout, _product, _platform, _virtualHost, _requestHeartbeat, _prefetchCount, _publisherConfirms, _persistentMessages);

            return new NetqRabbitRpcClient(connection, LoggerFactory);
        }

        public IRpcServer BuildServer()
        {
            INetqRabbitRpcConnection<Func<Type, object>> connection = InitConnection(_hostname, _password, _username, _rpcConfiguration, _serializer,
                _rpcMessageType, _timeout, _product, _platform, _virtualHost, _requestHeartbeat, _prefetchCount, _publisherConfirms, _persistentMessages);

            return new NetqRabbitRpcServer(connection, LoggerFactory);
        }
    }
}
