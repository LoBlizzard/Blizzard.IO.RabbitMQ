using EasyNetQ;
using System;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class ConcreteMessageSerializationStrategy : IMessageSerializationStrategy
    {
        private readonly ISerializer _serializer;

        public ConcreteMessageSerializationStrategy(ISerializer serializer)
        {
            _serializer = serializer;
        }

        /// <summary>
        /// This method is for internal use of the library.
        /// The user of the easynetq pipeline should expect to always receive <param type="Func<Type, object>"></param> when calling request or respond and to call 
        /// this lambda with the right type in order to get the actual object
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public IMessage DeserializeMessage(MessageProperties properties, byte[] body)
        {
            return new Message<Func<Type, object>>(type => _serializer.Deserialize(body, type), properties);
        }

        public SerializedMessage SerializeMessage(IMessage message)
        {
            return new SerializedMessage(message.Properties, _serializer.Serialize(message.GetBody()));
        }
    }
}
