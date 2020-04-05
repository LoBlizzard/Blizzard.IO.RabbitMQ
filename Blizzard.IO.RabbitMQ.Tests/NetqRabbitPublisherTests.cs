using Blizzard.IO.Core;
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
        private Mock<IConverter<RabbitMessageProperties, MessageProperties>> _converterMock;
        private Mock<ILoggerFactory> _loggerFactoryMock;
        private Mock<ILogger> _loggerMock;

        private RabbitExchange _defaultDestinationExchange;
        private MessageProperties _defaultMessageProperties;
        private RabbitMessageProperties _defaultRabbitMessageProperties;
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
            _converterMock = new Mock<IConverter<RabbitMessageProperties, MessageProperties>>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger>();

            _defaultSerializedData = new byte[0];
            _defaultDestinationExchange = new RabbitExchange()
            {
                Name = "test"
            };
            _defaultMessageProperties = new MessageProperties();
            _defaultRabbitMessageProperties = new RabbitMessageProperties();
            _defaultRoutingKey = "test";
            _defaultData = "data";
            _defaultIsAbstract = false;

            _serializerMock.Setup(serializer => serializer.Serialize(It.IsAny<string>())).Returns(_defaultSerializedData);
            _busMock.Setup(bus => bus.Advanced).Returns(_advancedBusMock.Object);
            _converterMock.Setup(converter => converter.Convert(_defaultRabbitMessageProperties))
                .Returns(_defaultMessageProperties);
            _loggerFactoryMock.Setup(loggerFactory => loggerFactory.CreateLogger(It.IsAny<string>()))
                .Returns(_loggerMock.Object);

            _netqRabbitPublisher = new NetqRabbitPublisher<string>(_busMock.Object, _serializerMock.Object,
                _defaultDestinationExchange, _loggerFactoryMock.Object, _converterMock.Object, _defaultIsAbstract,
                _defaultRoutingKey);
        }

        [Test]
        public void Publish_OnCalledWithDataOnly_ShouldCallSerializeAdvanceAndPublishOnce()
        {
            // Act
            _netqRabbitPublisher.Publish(_defaultData);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _converterMock.Verify(converter => converter.Convert(It.IsAny<RabbitMessageProperties>()), Times.Never);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), _defaultRoutingKey,
                false, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_OnCalledWithDataAndRoutingKey_ShouldCallSerializeAdvanceAndPublishOnce()
        {
            // Arrange
            string routingKey = "notDefault";

            // Act
            _netqRabbitPublisher.Publish(_defaultData, routingKey);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _converterMock.Verify(converter => converter.Convert(It.IsAny<RabbitMessageProperties>()), Times.Never);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), routingKey,
                false, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_OnCalledWithDataAndRabbitMessageProperties_ShouldCallSerializeAdvanceAndPublishOnce()
        {
            // Act
            _netqRabbitPublisher.Publish(_defaultData,_defaultRabbitMessageProperties);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _converterMock.Verify(converter => converter.Convert(_defaultRabbitMessageProperties), Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), _defaultRoutingKey,
                false, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_OnCalledWithDataAndRabbitMessagePropertiesAndRoutingKey_ShouldCallSerializeAdvanceAndPublishOnce()
        {
            // Arrange
            string routingKey = "notDefault";

            // Act
            _netqRabbitPublisher.Publish(_defaultData, _defaultRabbitMessageProperties, routingKey);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Once);
            _converterMock.Verify(converter => converter.Convert(_defaultRabbitMessageProperties), Times.Once);
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(It.IsAny<Exchange>(), routingKey,
                false, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_OnIsAbstractTrue_ShouldEnrichWithType()
        {
            // Arrange
            var abstractNetqRabbitPublisher = new NetqRabbitPublisher<string>(_busMock.Object, _serializerMock.Object,
                _defaultDestinationExchange, _loggerFactoryMock.Object, _converterMock.Object, true);

            // Act
            abstractNetqRabbitPublisher.Publish(_defaultData,_defaultRabbitMessageProperties);

            // Assert
            Assert.AreEqual(typeof(string).ToString(), _defaultMessageProperties.Type);
        }
    }
}
