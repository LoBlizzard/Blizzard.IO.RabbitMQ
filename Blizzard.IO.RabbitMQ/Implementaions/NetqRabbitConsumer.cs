using System;
using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Blizzard.IO.RabbitMQ.Implementaions
{
    public class NetqRabbitConsumer<TData> : IConsumer<TData>, IConsumerWithMetadata<TData, RabbitMessageProperties>
    {
        private readonly IBus _netqBus;
        private readonly IDeserializer<TData> _deserializer;
        private readonly IConcreteTypeDeserializer<TData> _concreteTypeDeserializer;
        private readonly IQueue _sourceQueue;

        public event Action<TData> MessageReceived;
        public event Action<TData, RabbitMessageProperties> MessageWithMetadataReceived;

        public NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue,
            IDeserializer<TData> deserializer)
        {
            _netqBus = netqBus ?? throw new ArgumentNullException(nameof(netqBus));
            _sourceQueue = sourceQueue ?? throw new ArgumentNullException(nameof(sourceQueue));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public NetqRabbitConsumer(IBus netqBus, IQueue sourceQueue,
            IConcreteTypeDeserializer<TData> concreteTypeDeserializer)
        {
            _netqBus = netqBus ?? throw new ArgumentNullException(nameof(netqBus));
            _sourceQueue = sourceQueue ?? throw new ArgumentNullException(nameof(sourceQueue));
            _concreteTypeDeserializer = concreteTypeDeserializer ?? throw new ArgumentNullException(nameof(concreteTypeDeserializer));
        }

        public void Start()
        {
            _netqBus.Advanced.Consume(_sourceQueue, (messageBytes, messageProperties, messageInfo) =>
            {
                if (_concreteTypeDeserializer != null)
                {
                    //TData data = _concreteTypeDeserializer.Deserialize()
                }
            })
        }

        public void Stop()
        {
            _netqBus.Dispose();
        }
    }
}
