using System;
using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ.Implementaions
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
        
        public event Action<TData> MessageReceived;
        public event Action<TData, RabbitMessageProperties> MessageWithMetadataReceived;

        protected NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue, ILoggerFactory loggerFactory,
            IConverter<MessageProperties, RabbitMessageProperties> coverter)
        {
            _netqBus = netqBus;
            _sourceQueue = sourceQueue;
            _coverter = coverter ?? new RabbitPropertiesConverter();
        }

        public NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue, IDeserializer<TData> deserializer, 
            ILoggerFactory loggerFactory, IConverter<MessageProperties, RabbitMessageProperties> coverter = null)
            : this(netqBus, sourceQueue, loggerFactory, coverter)
        {
            _deserializer = deserializer;
        }

        public NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue, IConcreteTypeDeserializer<TData> concreteTypeDeserializer,
            ILoggerFactory loggerFactory, IConverter<MessageProperties, RabbitMessageProperties> coverter = null)
            : this(netqBus, sourceQueue, loggerFactory, coverter)
        {
            _concreteTypeDeserializer = concreteTypeDeserializer;
        }

        public void Start()
        {
            _consumeTask = _netqBus.Advanced.Consume(_sourceQueue, (messageBytes, messageProperties, messageInfo) =>
            {
                TData data;
                if (_concreteTypeDeserializer != null)
                {
                    Type messageObjectType = messageProperties.Type.GetType();
                    data = _concreteTypeDeserializer.Deserialize(messageBytes, messageObjectType);
                }
                else
                {
                    data = _deserializer.Deserialize(messageBytes);
                }

                RabbitMessageProperties properties = _coverter.Convert(messageProperties);
                MessageReceived?.Invoke(data);
                MessageWithMetadataReceived?.Invoke(data, properties);
            });       
        }

        public void Stop()
        {
            if (_consumeTask == null)
            {

            }
            _consumeTask.Dispose();
        }
    }
}
