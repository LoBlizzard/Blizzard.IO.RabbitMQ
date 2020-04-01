    using EasyNetQ;
using System;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    internal class ConcreteMessageSerializationStrategy : IMessageSerializationStrategy
    {
        private readonly ISerializer _serializer;

        public ConcreteMessageSerializationStrategy(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public IMessage DeserializeMessage(MessageProperties properties, byte[] body)
        {
            return new Message<Func<Type,object>>(type=>_serializer.Deserialize(body, type), properties);
        }

        public SerializedMessage SerializeMessage(IMessage message)
        {
            return new SerializedMessage(message.Properties, _serializer.Serialize(message.GetBody()));
        }
    }
}
