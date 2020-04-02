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
        private readonly IQueue _sourceQueue;
        private readonly IConverter<MessageProperties, RabbitMessageProperties> _coverter;
        private readonly ILogger<NetqRabbitConsumer<TData>> _logger;
        
        private IDisposable _consumeTask;
        private readonly Action<byte[], MessageProperties, MessageReceivedInfo> _consumingStrategy;

        public event Action<TData> MessageReceived;
        public event Action<TData, RabbitMessageProperties> MessageWithMetadataReceived;

        protected NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue, ILoggerFactory loggerFactory,
            IConverter<MessageProperties, RabbitMessageProperties> coverter)
        {
            _netqBus = netqBus;
            _sourceQueue = sourceQueue;
            _logger = loggerFactory.CreateLogger<NetqRabbitConsumer<TData>>();
            _coverter = coverter ?? new RabbitPropertiesConverter();
        }

        public NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue, IDeserializer<TData> deserializer, 
            ILoggerFactory loggerFactory, IConverter<MessageProperties, RabbitMessageProperties> coverter = null)
            : this(netqBus, sourceQueue, loggerFactory, coverter)
        {
            _deserializer = deserializer;
            _consumingStrategy += ConsumeViaDeserializer;
            _logger.LogDebug($"Initialized consumer from queue: {_sourceQueue.Name}," +
                $" expecting {typeof(TData).ToString()} data");
        }

        public NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue, IConcreteTypeDeserializer<TData> concreteTypeDeserializer,
            ILoggerFactory loggerFactory, IConverter<MessageProperties, RabbitMessageProperties> coverter = null)
            : this(netqBus, sourceQueue, loggerFactory, coverter)
        {
            _concreteTypeDeserializer = concreteTypeDeserializer;
            _consumingStrategy += ConsumeViaConcreteDeserializer;
            _logger.LogDebug($"Initialized consumer from queue: {_sourceQueue.Name}," +
                $" expecting {typeof(TData).ToString()} derieved data");
        }

        public void Start()
        {
            _consumeTask = _netqBus.Advanced.Consume(_sourceQueue, _consumingStrategy);
            _logger.LogInformation("Started consumer sucessfully");
        }

        private void ConsumeViaDeserializer(byte[] messageBytes, MessageProperties messageProperties,
            MessageReceivedInfo messageInfo)
        {
            TData data = _deserializer.Deserialize(messageBytes);
            RabbitMessageProperties properties = _coverter.Convert(messageProperties);
            MessageReceived?.Invoke(data);
            MessageWithMetadataReceived?.Invoke(data, properties);
        }

        private void ConsumeViaConcreteDeserializer(byte[] messageBytes, MessageProperties messageProperties,
            MessageReceivedInfo messageInfo)
        {
            Type messageConcreteType = messageProperties.Type.GetType();
            TData data = _concreteTypeDeserializer.Deserialize(messageBytes, messageConcreteType);
            RabbitMessageProperties properties = _coverter.Convert(messageProperties);
            MessageReceived?.Invoke(data);
            MessageWithMetadataReceived?.Invoke(data, properties);
        }

        public void Stop()
        {
            if (_consumeTask == null)
            {
                _logger.LogInformation("Couldn't stop consumer, please make sure it was started");
            }
            else
            {
                _consumeTask.Dispose();
                _consumeTask = null;
                _logger.LogInformation("Stopped consuming data succesfully");
            }
        }
    }
}
