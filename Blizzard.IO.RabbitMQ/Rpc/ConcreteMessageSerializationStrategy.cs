using EasyNetQ;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    internal class ConcreteMessageSerializationStrategy : IMessageSerializationStrategy
    {
        private readonly ISerializer _serializer;

        public IMessage DeserializeMessage(MessageProperties properties, byte[] body)
        {
            return MessageFactory.CreateInstance(typeof(byte[]), body, properties);
        }

        public SerializedMessage SerializeMessage(IMessage message)
        {
            return new SerializedMessage(message.Properties, _serializer.Serialize(message.GetBody()));
        }
    }
}
