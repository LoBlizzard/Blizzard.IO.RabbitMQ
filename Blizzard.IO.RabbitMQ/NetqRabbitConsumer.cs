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
        private readonly IConcreteTypeDeserializer<TData> _concreteTypeDeserializer;
        private readonly IQueue _netqQueue;
        private readonly IConverter<MessageProperties, RabbitMessageProperties> _converter;
        private readonly ILogger<NetqRabbitConsumer<TData>> _logger;
        
        private IDisposable _consumeHandler;
        private readonly Func<byte[], MessageProperties, TData> _dataExtractionStrategy;

        public event Action<TData> MessageReceived;
        public event Action<TData, RabbitMessageProperties> MessageWithMetadataReceived;

        protected NetqRabbitConsumer(IBus netqBus, RabbitQueue sourceQueue, ILoggerFactory loggerFactory,
            IConverter<MessageProperties, RabbitMessageProperties> coverter)
        {
            _netqBus = netqBus;
            _netqQueue = new Queue(sourceQueue.Name, sourceQueue.Exclusive);
            _logger = loggerFactory.CreateLogger<NetqRabbitConsumer<TData>>();
            _converter = coverter ?? new RabbitPropertiesConverter();
        }

        public NetqRabbitConsumer(IBus netqBus, RabbitQueue sourceQueue, IDeserializer<TData> deserializer, 
            ILoggerFactory loggerFactory, IConverter<MessageProperties, RabbitMessageProperties> coverter = null)
            : this(netqBus, sourceQueue, loggerFactory, coverter)
        {
            _deserializer = deserializer;
            _dataExtractionStrategy += ExtractData;
            _logger.LogDebug($"Initialized consumer from queue: {_netqQueue.Name}," +
                $" expecting {typeof(TData)} data");
        }

        public NetqRabbitConsumer(IBus netqBus, RabbitQueue sourceQueue, IConcreteTypeDeserializer<TData> concreteTypeDeserializer,
            ILoggerFactory loggerFactory, IConverter<MessageProperties, RabbitMessageProperties> coverter = null)
            : this(netqBus, sourceQueue, loggerFactory, coverter)
        {
            _concreteTypeDeserializer = concreteTypeDeserializer;
            _dataExtractionStrategy += ExtractAbstractData;
            _logger.LogDebug($"Initialized consumer from queue: {_netqQueue.Name}," +
                $" expecting {typeof(TData)} derieved data");
        }

        public void Start()
        {
            _consumeHandler = _netqBus.Advanced.Consume(_netqQueue, (messageBytes, messageProperties, messageIndo) =>
            {
                TData data = _dataExtractionStrategy.Invoke(messageBytes, messageProperties);
                RabbitMessageProperties properties = _converter.Convert(messageProperties);
                MessageReceived?.Invoke(data);
                MessageWithMetadataReceived?.Invoke(data, properties);
            });

            _logger.LogInformation("Started consumer sucessfully");
        }

        private TData ExtractData(byte[] messageBytes, MessageProperties messageProperties)
        {
            return _deserializer.Deserialize(messageBytes);
        }

        private TData ExtractAbstractData(byte[] messageBytes, MessageProperties messageProperties)
        {
            Type messageConcreteType = Type.GetType(messageProperties.Type);
            return _concreteTypeDeserializer.Deserialize(messageBytes, messageConcreteType);
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
