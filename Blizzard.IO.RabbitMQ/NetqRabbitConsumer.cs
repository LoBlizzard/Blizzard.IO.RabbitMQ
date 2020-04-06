using System;
using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ
{
    public class NetqRabbitConsumer<TData> : IConsumer<TData>, IConsumerWithMetadata<TData, RabbitMessageProperties>
    {
        private readonly IBus _netqBus;
        private readonly IDeserializer<TData> _deserializer;
        private readonly IAbstractTypeDeserializer<TData> _abstractDeserializer;
        private readonly IQueue _netqQueue;
        private readonly IConverter<MessageProperties, RabbitMessageProperties> _messagePropertiesToRabbitMessagePropertiesConverter;
        private readonly ILogger _logger;
        
        private IDisposable _consumeHandler;
        private readonly Func<byte[], MessageProperties, TData> _dataExtractionStrategy;

        public event Action<TData> MessageReceived;
        public event Action<TData, RabbitMessageProperties> MessageWithMetadataReceived;

        protected NetqRabbitConsumer(IBus netqBus, RabbitQueue sourceQueue, ILoggerFactory loggerFactory,
            IConverter<MessageProperties, RabbitMessageProperties> messagePropertiesToRabbitMessagePropertiesConverter)
        {
            _netqBus = netqBus;
            _netqQueue = new Queue(sourceQueue.Name, sourceQueue.Exclusive);
            _logger = loggerFactory.CreateLogger(nameof(NetqRabbitConsumer<TData>));
            _messagePropertiesToRabbitMessagePropertiesConverter = messagePropertiesToRabbitMessagePropertiesConverter ?? new RabbitPropertiesConverter();
        }

        public NetqRabbitConsumer(IBus netqBus, RabbitQueue sourceQueue, IDeserializer<TData> deserializer, ILoggerFactory loggerFactory,
            IConverter<MessageProperties, RabbitMessageProperties> messagePropertiesToRabbitMessagePropertiesConverter = null)
            : this(netqBus, sourceQueue, loggerFactory, messagePropertiesToRabbitMessagePropertiesConverter)
        {
            _deserializer = deserializer;
            _dataExtractionStrategy += ExtractConcreteData;
            _logger.LogDebug($"Initialized consumer from queue: {_netqQueue.Name}," +
                $" expecting {typeof(TData)} data");
        }

        public NetqRabbitConsumer(IBus netqBus, RabbitQueue sourceQueue, IAbstractTypeDeserializer<TData> abstractDeserializer,
            ILoggerFactory loggerFactory,
            IConverter<MessageProperties, RabbitMessageProperties> messagePropertiesToRabbitMessagePropertiesConverter = null)
            : this(netqBus, sourceQueue, loggerFactory, messagePropertiesToRabbitMessagePropertiesConverter)
        {
            _abstractDeserializer = abstractDeserializer;
            _dataExtractionStrategy += ExtractAbstractData;
            _logger.LogDebug($"Initialized consumer from queue: {_netqQueue.Name}," +
                $" expecting {typeof(TData)} derieved data");
        }

        public void Start()
        {
            if (MessageReceived == null && MessageWithMetadataReceived == null)
            {
                _logger.LogWarning("Couldn't start consumer, must have at least one subscriber in order to start");
            }
            else
            {
                _consumeHandler = _netqBus.Advanced.Consume(_netqQueue, (messageBytes, messageProperties, messageIndo) =>
                {
                    TData data = _dataExtractionStrategy(messageBytes, messageProperties);
                    RabbitMessageProperties properties = _messagePropertiesToRabbitMessagePropertiesConverter.Convert(messageProperties);
                    MessageReceived?.Invoke(data);
                    MessageWithMetadataReceived?.Invoke(data, properties);
                });

                _logger.LogInformation("Started consumer sucessfully");
            }
        }

        private TData ExtractConcreteData(byte[] messageBytes, MessageProperties messageProperties)
        {
            return _deserializer.Deserialize(messageBytes);
        }

        private TData ExtractAbstractData(byte[] messageBytes, MessageProperties messageProperties)
        {
            Type messageConcreteType = Type.GetType(messageProperties.Type);
            return _abstractDeserializer.Deserialize(messageBytes, messageConcreteType);
        }

        public void Stop()
        {
            if (_consumeHandler == null)
            {
                _logger.LogWarning("Couldn't stop consumer, please make sure it was started");
            }
            else
            {
                _consumeHandler.Dispose();
                _consumeHandler = null;
                _logger.LogInformation($"Stopped consuming data from queue: {_netqQueue.Name} succesfully");
            }
        }
    }
}
