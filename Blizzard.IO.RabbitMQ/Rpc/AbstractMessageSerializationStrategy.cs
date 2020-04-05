using EasyNetQ;
using System;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class AbstractMessageSerializationStrategy : IMessageSerializationStrategy
    {
        private readonly ISerializer _serializer;
        private readonly ITypeNameSerializer _typeNameSerializer;

        public AbstractMessageSerializationStrategy(ISerializer serializer, ITypeNameSerializer typeNameSerializer)
        {
            _serializer = serializer;
            _typeNameSerializer = typeNameSerializer;
        }

        public IMessage DeserializeMessage(MessageProperties properties, byte[] body)
        {
            Type type = _typeNameSerializer.DeSerialize(properties.Type);
            object obj = _serializer.Deserialize(body, type);
            return MessageFactory.CreateInstance(type, obj);
        }

        public SerializedMessage SerializeMessage(IMessage message)
        {
            string type = _typeNameSerializer.Serialize(message.MessageType);
            byte[] body = _serializer.Serialize(message.GetBody());
            MessageProperties messageProperties = message.Properties;
            messageProperties.Type = type;

            return new SerializedMessage(messageProperties, body);
        }
    }
}
