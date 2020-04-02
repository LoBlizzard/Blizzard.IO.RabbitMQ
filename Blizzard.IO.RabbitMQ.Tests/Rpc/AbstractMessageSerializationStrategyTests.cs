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
    public class AbstractMessageSerializationStrategyTests
    {
        private Mock<ISerializer> _serializerMock;
        private Mock<ITypeNameSerializer> _typeNameSerializerMock;

        private AbstractMessageSerializationStrategy _abstractMessageSerializationStrategy;

        [SetUp]
        public void Setup()
        {
            _serializerMock = new Mock<ISerializer>();
            _typeNameSerializerMock = new Mock<ITypeNameSerializer>();
            _abstractMessageSerializationStrategy = new AbstractMessageSerializationStrategy(_serializerMock.Object, _typeNameSerializerMock.Object);
        }

        [Test]
        public void SerializeMessage_OnValidIMessage_ShouldReturnsValidSerializedMessageWithTheSerializedDataAndEnrichedProperties()
        {
            //Arrange
            long data = 1000;

            var serializedData = new byte[]
            {
                0,1,2,3,4,5
            };

            _serializerMock.Setup(sr => sr.Serialize(It.IsAny<long>())).Returns(serializedData);
            _typeNameSerializerMock.Setup(tns => tns.Serialize(It.IsAny<Type>())).Returns(typeof(long).ToString());
            MessageProperties messageProperties = new MessageProperties();


            IMessage message = new Message<long>(data, messageProperties);

            //Act
            SerializedMessage output = _abstractMessageSerializationStrategy.SerializeMessage(message);

            //Assert
            Assert.True(serializedData.SequenceEqual(output.Body));
            Assert.True(output.Properties == messageProperties);
            Assert.AreEqual(output.Properties.Type, typeof(long).ToString());
        }

        [Test]
        public void SerializeMessage_OnValidIMessage_ShouldCallTheCorrectDependencies()
        {
            //Arrange
            long data = 1000;

            var serializedData = new byte[]
            {
                0,1,2,3,4,5
            };

            _serializerMock.Setup(sr => sr.Serialize(It.IsAny<long>())).Returns(serializedData);
            _typeNameSerializerMock.Setup(tns => tns.Serialize(It.IsAny<Type>())).Returns("alon");
            MessageProperties messageProperties = new MessageProperties();


            IMessage message = new Message<long>(data, messageProperties);

            //Act
            SerializedMessage output = _abstractMessageSerializationStrategy.SerializeMessage(message);

            //Assert
            _typeNameSerializerMock.Verify(serializer => serializer.Serialize(It.IsAny<Type>()));
            _serializerMock.Verify(serializer => serializer.Serialize(It.IsAny<object>()));
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

            _serializerMock.Setup(sr => sr.Deserialize(It.IsAny<byte[]>(), It.IsAny<Type>())).Returns(data);
            _typeNameSerializerMock.Setup(tns => tns.DeSerialize(It.IsAny<string>())).Returns(typeof(long));
            MessageProperties messageProperties = new MessageProperties();

            //Act
            Message<long> output = (Message<long>)_abstractMessageSerializationStrategy
                .DeserializeMessage(messageProperties, serializedData);

            //Assert
            var outputData = (long)output.Body;
            Assert.True(data == outputData);
        }
    }
}
