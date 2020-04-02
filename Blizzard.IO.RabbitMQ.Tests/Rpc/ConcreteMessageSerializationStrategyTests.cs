using Blizzard.IO.RabbitMQ.Rpc;
using EasyNetQ;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;

namespace Blizzard.IO.RabbitMQ.Tests.Rpc
{
    [TestFixture]
    public class ConcreteMessageSerializationStrategyTests
    {
        private Mock<ISerializer> _serializerMock;
        private ConcreteMessageSerializationStrategy _concreteMessageSerializationStrategy;

        [SetUp]
        public void Setup()
        {
            _serializerMock = new Mock<ISerializer>();
            _concreteMessageSerializationStrategy = new ConcreteMessageSerializationStrategy(_serializerMock.Object);
        }

        [Test]
        public void SerializeMessage_OnValidIMessage_ShouldReturnsValidSerializedMessage()
        {
            //Arrange
            long data = 1000;

            var serializedData = new byte[]
            {
                0,1,2,3,4,5
            };

            _serializerMock.Setup(sr => sr.Serialize(It.IsAny<long>())).Returns(serializedData);
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

            var serializedData = new byte[]
            {
                0,1,2,3,4,5
            };

            _serializerMock.Setup(sr => sr.Deserialize(It.IsAny<byte[]>(),It.IsAny<Type>())).Returns(data);
            MessageProperties messageProperties = new MessageProperties();

            //Act
            Message<Func<Type, object>> output = (Message<Func<Type, object>>)_concreteMessageSerializationStrategy
                .DeserializeMessage(messageProperties, serializedData);

            //Assert
            var outputData = (long)output.Body(typeof(long));
            Assert.True(data == outputData);
        }
    }
}
