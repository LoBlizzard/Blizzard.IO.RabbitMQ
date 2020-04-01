using Blizzard.IO.RabbitMQ.Rpc;
using EasyNetQ;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;

namespace Blizzard.IO.RabbitMQ.Tests.Rpc
{
    public class AbstractMessageSerializationStrategyTests
    {
        private Mock<ISerializer> _serializerMock;
        private Mock<ITypeNameSerializer> _typeNameSerializerMock;

        private AbstractMessageSerializationStrategy _concreteMessageSerializationStrategy;

        [SetUp]
        public void Setup()
        {
            _serializerMock = new Mock<ISerializer>();
            _typeNameSerializerMock = new Mock<ITypeNameSerializer>();
            _concreteMessageSerializationStrategy = new AbstractMessageSerializationStrategy(_serializerMock.Object, _typeNameSerializerMock.Object);
        }

        [Test]
        public void SerializeMessage_OnValidIMessage_ShouldReturnsValidSerializedMessage()
        {
            //Arrange
            long data = 1000;

            byte[] serializedData = new byte[]
            {
                0,1,2,3,4,5
            };

            _serializerMock.Setup(sr => sr.Serialize(It.IsAny<long>())).Returns(serializedData);
            _typeNameSerializerMock.Setup(tns => tns.Serialize(It.IsAny<Type>())).Returns("alon");
            MessageProperties messageProperties = new MessageProperties();


            IMessage message = new Message<long>(data, messageProperties);

            //Act
            SerializedMessage output = _concreteMessageSerializationStrategy.SerializeMessage(message);

            //Assert
            Assert.True(serializedData.SequenceEqual(output.Body));
            Assert.True(output.Properties == messageProperties);
        }

        [Test]
        public void DeserializeMessage_OnValidMessagePropertiesAndData_ShouldReturnValidMessageWithValidFunc()
        {
            //Arrange
            long data = 1000;

            byte[] serializedData = new byte[]
            {
                0,1,2,3,4,5
            };

            _serializerMock.Setup(sr => sr.Deserialize(It.IsAny<byte[]>(), It.IsAny<Type>())).Returns(data);
            _typeNameSerializerMock.Setup(tns => tns.DeSerialize(It.IsAny<string>())).Returns(typeof(long));
            MessageProperties messageProperties = new MessageProperties();

            //Act
            Message<long> output = (Message<long>)_concreteMessageSerializationStrategy
                .DeserializeMessage(messageProperties, serializedData);

            //Assert
            var outputData = (long)output.Body;
            Assert.True(data == outputData);
        }
    }
}
