using System;
using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;

namespace Blizzard.IO.RabbitMQ.Implementaions
{
    public class RabbitConsumer<TData> : IConsumer<TData>, IConsumerWithMetadata<TData, RabbitMessageProperties>
    {
        public event Action<TData> MessageReceived;
        public event Action<TData, RabbitMessageProperties> MessageWithMetadataReceived;

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
