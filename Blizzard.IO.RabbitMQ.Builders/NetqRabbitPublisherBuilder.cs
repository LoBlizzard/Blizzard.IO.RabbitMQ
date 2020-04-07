using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using Blizzard.IO.RabbitMQ.Extensions;
using Blizzard.IO.Serialization.Json;
using EasyNetQ;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ.Builders
{
    public class NetqRabbitPublisherBuilder<TData> : BaseNetqRabbitBuilder
    {
        private RabbitExchange _destinationExchange = null;
        private ISerializer<TData> _serializer = new JsonSerializer<TData>();
        private string _routingKey = "/";
        private bool _isAbstract;
        private IConverter<RabbitMessageProperties, MessageProperties> _converter;
        private ILoggerFactory _loggerFactory;

        public NetqRabbitPublisherBuilder(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public NetqRabbitPublisherBuilder<TData> AddRabbitConnectionProperties(
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

        public NetqRabbitPublisherBuilder<TData> AddHost(string host)
        {
            Hostname = host;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddCredentials(string username, string password)
        {
            Username = username;
            Password = password;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddHostAndCredentials(string host, string username, string password)
        {
            return AddHost(host).AddCredentials(username, password);
        }

        public NetqRabbitPublisherBuilder<TData> AddSerializer(ISerializer<TData> serializer)
        {
            _serializer = serializer;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddAbstractSupport()
        {
            _isAbstract = true;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddDestinationExchange(string name, RabbitExchangeType type)
        {
            _destinationExchange = new RabbitExchange()
            {
                Name = name, 
                Type = type
            };
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddDestinationExchange(RabbitExchange exchange)
        {
            _destinationExchange = exchange;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddRoutingKey(string routingKey)
        {
            _routingKey = routingKey;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddConverter(
            IConverter<RabbitMessageProperties, MessageProperties> converter)
        {
            _converter = converter;
            return this;
        }

        public NetqRabbitPublisher<TData> BuildPublisher()
        {
            IBus bus = InitConnection();

            if (_destinationExchange == null)
            {
                _destinationExchange = bus.DeclareExchange("DefaultExchange", RabbitExchangeType.Fanout);
            }

            return new NetqRabbitPublisher<TData>(bus, _serializer, _destinationExchange, _loggerFactory, _converter,
                _isAbstract, _routingKey);
        }
    }
}