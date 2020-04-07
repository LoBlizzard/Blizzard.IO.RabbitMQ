using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using Blizzard.IO.RabbitMQ.Extensions;
using Blizzard.IO.Serialization.Json;
using EasyNetQ;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ.Builders
{
    public class NetqRabbitConsumerBuilder<TData> : BaseNetqRabbitBuilder
    {
        private RabbitQueue _sourceQueue;
        private IDeserializer<TData> _deserializer = new JsonSerializer<TData>();
        private IAbstractTypeDeserializer<TData> _abstractDeserializer = new JsonAbstractTypeDeserializer<TData>();
        private IConverter<MessageProperties, RabbitMessageProperties> _converter;
        private bool _isAbstract = false;

        public NetqRabbitConsumerBuilder(ILoggerFactory loggerFactory) 
            : base(loggerFactory)
        {
        }

        public NetqRabbitConsumerBuilder<TData> AddRabbitConnectionProperties(
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

        public NetqRabbitConsumerBuilder<TData> AddHostnameAndCredentials(string hostname, string username, string password)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
            return this;
        }

        public NetqRabbitConsumerBuilder<TData> AddCredentials(string username, string password)
        {
            Username = username;
            Password = password;
            return this;
        }

        public NetqRabbitConsumerBuilder<TData> AddDeserializer(IDeserializer<TData> deserializer)
        {
            _deserializer = deserializer;
            return this;
        }

        public NetqRabbitConsumerBuilder<TData> AddAbstractDeserializer(IAbstractTypeDeserializer<TData> abstractDeserializer)
        {
            _abstractDeserializer = abstractDeserializer;
            return this;
        }

        public NetqRabbitConsumerBuilder<TData> AddAbstractSupport()
        {
            _isAbstract = true;
            return this;
        }

        public NetqRabbitConsumerBuilder<TData> RemoveAbstractSupport()
        {
            _isAbstract = false;
            return this;
        }

        public NetqRabbitConsumerBuilder<TData> AddSourceQueue(RabbitQueue sourceQueue)
        {
            _sourceQueue = sourceQueue;
            return this;
        }

        public NetqRabbitConsumerBuilder<TData> AddConverter(IConverter<MessageProperties, RabbitMessageProperties> converter)
        {
            _converter = converter;
            return this;
        }

        public NetqRabbitConsumer<TData> Build()
        {
            IBus bus = InitConnection();
            _sourceQueue = _sourceQueue ?? bus.DeclareQueue("DefaultQueue");
            if (_isAbstract)
            {
                return new NetqRabbitConsumer<TData>(bus, _sourceQueue, _abstractDeserializer, LoggerFactory, _converter);
            }

            return new NetqRabbitConsumer<TData>(bus, _sourceQueue, _deserializer, LoggerFactory, _converter);
        }
    }
}
