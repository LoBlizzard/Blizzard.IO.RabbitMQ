using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;
using Moq;
using NUnit.Framework;

namespace Blizzard.IO.RabbitMQ.Tests
{
    [TestFixture]
    class NetqRabbitPublisherTests
    {
        private Mock<IBus> _busMock;
        private Mock<Core.ISerializer<string>> _serializerMock;
        private Mock<IAdvancedBus> _advancedBusMock;
        private Mock<IExchange> _exchangeMock;

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
            _exchangeMock = new Mock<IExchange>();

            _defaultSerializedData = new byte[0];
            _defaultDestinationExchange = new RabbitExchange()
            {
                Name = "test",
                Type = RabbitExchangeType.Fanout
            };
            _defaultRoutingKey = "test";
            _defaultData = "data";
            _defaultIsAbstract = false;

            _serializerMock.Setup(serializer => serializer.Serialize(It.IsAny<string>())).Returns(_defaultSerializedData);
            _busMock.Setup(bus => bus.Advanced).Returns(_advancedBusMock.Object);
            _advancedBusMock.Setup(advancedBus => advancedBus.ExchangeDeclare(_defaultDestinationExchange.Name, Utilities.ExchangeTypeToStringResolver[_defaultDestinationExchange.Type],
                _defaultDestinationExchange.Passive, _defaultDestinationExchange.Durable, _defaultDestinationExchange.AutoDelete,
                _defaultDestinationExchange.Internal, _defaultDestinationExchange.AlternateExchange, _defaultDestinationExchange.Delayed)).Returns(_exchangeMock.Object);

            _netqRabbitPublisher = new NetqRabbitPublisher<string>(_busMock.Object, _serializerMock.Object, _defaultDestinationExchange, _defaultIsAbstract, _defaultRoutingKey);
        }

        [Test]
        public void Publish_WithDataOnly_CallsSerializeAndPublishOnceAdvancedTwice()
        {
            // Act
            _netqRabbitPublisher.Publish(_defaultData);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Exactly(2));
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(_exchangeMock.Object, _defaultRoutingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_CalledWithDataAndRoutingKey_CallsSerializeAndPublishOnceAdvancedTwice()
        {
            // Arrange
            string routingKey = "notDefault";

            // Act
            _netqRabbitPublisher.Publish(_defaultData, routingKey);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Exactly(2));
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(_exchangeMock.Object, routingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_CalledWithDataAndRabbitMessageProperties_CallsSerializeAndPublishOnceAdvancedTwice()
        {
            // Act
            _netqRabbitPublisher.Publish(_defaultData, new RabbitMessageProperties());

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Exactly(2));
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(_exchangeMock.Object, _defaultRoutingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }

        [Test]
        public void Publish_CalledWithDataAndRabbitMessagePropertiesAndRoutingKey_CallsSerializeAndPublishOnceAdvancedTwice()
        {
            // Arrange
            string routingKey = "notDefault";

            // Act
            _netqRabbitPublisher.Publish(_defaultData, new RabbitMessageProperties(), routingKey);

            // Assert
            _serializerMock.Verify(serializer => serializer.Serialize(_defaultData), Times.Once);
            _busMock.Verify(bus => bus.Advanced, Times.Exactly(2));
            _advancedBusMock.Verify(advancedBus => advancedBus.Publish(_exchangeMock.Object, routingKey,
                _defaultIsAbstract, It.IsAny<MessageProperties>(), _defaultSerializedData), Times.Once);
        }
    }
}
