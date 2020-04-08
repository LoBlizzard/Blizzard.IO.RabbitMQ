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
        private const string DEFAULT_DESTINATION_EXCHANGE_NAME = "DEFAULT_EXCHANGE";
        private RabbitExchange _destinationExchange = null;
        private ISerializer<TData> _serializer = new JsonSerializer<TData>();
        private string _routingKey = "/";
        private bool _isAbstract = false;
        private IConverter<RabbitMessageProperties, MessageProperties> _converter;

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
        
        public NetqRabbitPublisherBuilder<TData> AddCredentials(string username, string password)
        {
            Username = username;
            Password = password;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddHostAndCredentials(string host, string username, string password)
        {
            Hostname = host;
            return AddCredentials(username, password);
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

        public NetqRabbitPublisherBuilder<TData> RemoveAbstractSupport()
        {
            _isAbstract = false;
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

        public NetqRabbitPublisherBuilder<TData> AddConverter(
            IConverter<RabbitMessageProperties, MessageProperties> converter)
        {
            _converter = converter;
            return this;
        }

        public NetqRabbitPublisher<TData> Build()
        {
            IBus bus = InitConnection();
            RabbitExchange destinationExchange = _destinationExchange ??
                bus.DeclareExchange(DEFAULT_DESTINATION_EXCHANGE_NAME, RabbitExchangeType.Fanout);

            return new NetqRabbitPublisher<TData>(bus, _serializer, destinationExchange, LoggerFactory, _converter,
                _isAbstract, _routingKey);
        }
    }
}