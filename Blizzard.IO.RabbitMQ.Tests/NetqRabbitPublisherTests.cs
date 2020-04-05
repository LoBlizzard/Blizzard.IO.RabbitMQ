using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ISerializer = Blizzard.IO.Core.ISerializer<string>;

namespace Blizzard.IO.RabbitMQ.Tests
{
    [TestFixture]
    class NetqRabbitPublisherTests
    {
        private Mock<IBus> _busMock;
        private Mock<ISerializer> _serializerMock;
        private Mock<IAdvancedBus> _advancedBusMock;
        private Mock<ILoggerFactory> _loggerFactoryMock;
        private Mock<ILogger> _loggerMock;

        private RabbitExchange _defaultDestinationExchange;
        private byte[] _defaultSerializedData;
        private string _defaultRoutingKey;
        private string _defaultData;
        private bool _defaultIsAbstract;

        private NetqRabbitPublisher<string> _netqRabbitPublisher;

        [SetUp]
        public void SetUp()
        {
            _busMock = new Mock<IBus>();
            _serializerMock = new Mock<Core.ISerializer<string>>();
            _advancedBusMock = new Mock<IAdvancedBus>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger>();

            _defaultSerializedData = new byte[0];
            _defaultDestinationExchange = new RabbitExchange()
            {
                Name = "test"
            };
            _defaultRoutingKey = "test";
            _defaultData = "data";
            _defaultIsAbstract = false;

            _serializerMock.Setup(serializer => serializer.Serialize(It.IsAny<string>())).Returns(_defaultSerializedData);
            _busMock.Setup(bus => bus.Advanced).Returns(_advancedBusMock.Object);
            _loggerFactoryMock.Setup(loggerFactory => loggerFactory.CreateLogger(It.IsAny<string>()))
                .Returns(_loggerMock.Object);

            _netqRabbitPublisher = new NetqRabbitPublisher<string>(_busMock.Object, _serializerMock.Object, _defaultDestinationExchange, _loggerFactoryMock.Object, _defaultIsAbstract, _defaultRoutingKey);
        }

        [Test]
        public void Publish_WithDataOnly_CallsSerializeAdvanceAndPublishOnce()
        {
            // Act
            _netqRabbitPublisher.Publish(_defaultData);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), _defaultRoutingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_CalledWithDataAndRoutingKey_CallsSerializeAdvanceAndPublishOnce()
        {
            // Arrange
            string routingKey = "notDefault";

            // Act
            _netqRabbitPublisher.Publish(_defaultData, routingKey);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), routingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_CalledWithDataAndRabbitMessageProperties_CallsSerializeAdvanceAndPublishOnce()
        {
            // Act
            _netqRabbitPublisher.Publish(_defaultData, new RabbitMessageProperties());

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), _defaultRoutingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_CalledWithDataAndRabbitMessagePropertiesAndRoutingKey_CallsSerializeAdvanceAndPublishOnce()
        {
            // Arrange
            string routingKey = "notDefault";

            // Act
            _netqRabbitPublisher.Publish(_defaultData, new RabbitMessageProperties(), routingKey);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), routingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }
    }
}
