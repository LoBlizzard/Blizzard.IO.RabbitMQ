using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;

namespace Blizzard.IO.RabbitMQ.Tests
{
    [TestFixture]
    public class NetqRabbitConsumerTests
    {
        private Mock<IBus> _netqBusMock;
        private Mock<IAdvancedBus> _netqAdvancedBusMock;
        private Mock<IAbstractTypeDeserializer<int>> _abstractDeserializerMock;
        private Mock<IDeserializer<int>> _deserializerMock;
        private Mock<IConverter<MessageProperties, RabbitMessageProperties>> _converterMock;
        private Mock<ILoggerFactory> _loggerFactoryMock;
        private Mock<ILogger> _loggerMock;
        
        private RabbitQueue _sourceQueue;

        private NetqRabbitConsumer<int> _netqRabbitConsumer;

        [SetUp]
        public void SetUp()
        {
            _netqBusMock = new Mock<IBus>();
            _netqAdvancedBusMock = new Mock<IAdvancedBus>();
            _deserializerMock = new Mock<IDeserializer<int>>();
            _abstractDeserializerMock = new Mock<IAbstractTypeDeserializer<int>>();
            _converterMock = new Mock<IConverter<MessageProperties, RabbitMessageProperties>>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger>();

            _sourceQueue = new RabbitQueue { Name = "EXAMPLE", Exclusive = false};

            _loggerFactoryMock.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
            _netqBusMock.SetupGet(bus => bus.Advanced).Returns(_netqAdvancedBusMock.Object);
        }

        private void SetUpConcreteConsumer()
        {
            _netqRabbitConsumer = new NetqRabbitConsumer<int>(_netqBusMock.Object, _sourceQueue,
                _deserializerMock.Object, _loggerFactoryMock.Object, _converterMock.Object);
        }

        private void SetUpAbstractConsumer()
        {
            _netqRabbitConsumer = new NetqRabbitConsumer<int>(_netqBusMock.Object, _sourceQueue,
                _abstractDeserializerMock.Object, _loggerFactoryMock.Object, _converterMock.Object);
        }

        [Test]
        public void Start_OnSubscriberIsFoundOnMessageReceived_ShouldTriggerConsumeOnce()
        {
            // Arrange
            SetUpConcreteConsumer();
            _netqRabbitConsumer.MessageReceived += x => { };
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()));

            // Act
            _netqRabbitConsumer.Start();

            // Assert
            _netqAdvancedBusMock.Verify(bus =>
                bus.Consume(It.IsAny<IQueue>(), It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()),
                Times.Once);
        }

        [Test]
        public void Start_OnSubscriberIsFoundOnMessageWithMEtadataReceived_ShouldTriggerConsumeOnce()
        {
            // Arrange
            SetUpConcreteConsumer();
            _netqRabbitConsumer.MessageWithMetadataReceived += (x, y) => { };
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()));

            // Act
            _netqRabbitConsumer.Start();

            // Assert
            _netqAdvancedBusMock.Verify(bus =>
                bus.Consume(It.IsAny<IQueue>(), It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()),
                Times.Once);
        }

        [Test]
        public void Start_OnBothConsumerEventsHasZeroSubscribers_ShouldThrowInvalidOperationException()
        {
            // Arrange
            SetUpConcreteConsumer();
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()));

            // Act + Assert
            Assert.That(() => _netqRabbitConsumer.Start(), Throws.TypeOf<InvalidOperationException>());            
        }

        [Test]
        public void Start_OnTryingToStartConsumerTwice_ShouldThrowInvalidOperationException()
        {
            // Arrange
            SetUpConcreteConsumer();
            _netqRabbitConsumer.MessageReceived += x => { };
            var disposableMock = new Mock<IDisposable>();
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>())).Returns(disposableMock.Object);
            _netqRabbitConsumer.Start();

            // Act + Assert
            Assert.That(() => _netqRabbitConsumer.Start(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ExtractConcreteData_OnConcreteDataExtractionStrategy_ShouldUseDeserializerAndConverterBeforeTriggeringEvents()
        {
            // Arrange
            SetUpConcreteConsumer();
            var messageReceivedEventCalled = false;
            var messageWithMetadataReceivedCalled = false;
            
            _netqRabbitConsumer.MessageReceived += x => messageReceivedEventCalled = true;
            _netqRabbitConsumer.MessageWithMetadataReceived += (x, y) => messageWithMetadataReceivedCalled = true;
            var dataBytes = new byte[] { 1, 2, 3, 4, 5 };
            var dataProperties = new MessageProperties();
            var dataInfo = new MessageReceivedInfo();

            Action<byte[], MessageProperties, MessageReceivedInfo> consumeAction = null;
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()))
                .Callback((IQueue queue, Action<byte[], MessageProperties, MessageReceivedInfo> action) => consumeAction = action);

            _netqRabbitConsumer.Start();

            // Act
            consumeAction(dataBytes, dataProperties, dataInfo);

            // Assert
            _deserializerMock.Verify(deserializer => deserializer.Deserialize(dataBytes), Times.Once);
            _converterMock.Verify(converter => converter.Convert(dataProperties), Times.Once);
            Assert.That(messageReceivedEventCalled, Is.True);
            Assert.That(messageWithMetadataReceivedCalled, Is.True);
        }

        [Test]
        public void ExtractAbstractData_OnAbstractDataExtractionStrategy_ShouldUseAbstractDeserializerAndConverterBeforeTriggeringEvents()
        {
            // Arrange
            SetUpAbstractConsumer();
            var messageReceivedEventCalled = false;
            var messageWithMetadataReceivedCalled = false;

            _netqRabbitConsumer.MessageReceived += x => messageReceivedEventCalled = true;
            _netqRabbitConsumer.MessageWithMetadataReceived += (x, y) => messageWithMetadataReceivedCalled = true;
            var dataBytes = new byte[] { 1, 2, 3, 4, 5 };
            var dataProperties = new MessageProperties { Type = typeof(int).ToString() };
            var dataInfo = new MessageReceivedInfo();

            Action<byte[], MessageProperties, MessageReceivedInfo> consumeAction = null;
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()))
                .Callback((IQueue queue, Action<byte[], MessageProperties, MessageReceivedInfo> action) => consumeAction = action);

            _netqRabbitConsumer.Start();

            // Act
            consumeAction(dataBytes, dataProperties, dataInfo);

            // Assert
            _abstractDeserializerMock.Verify(deserializer => deserializer.Deserialize(dataBytes, typeof(int)), Times.Once);
            _converterMock.Verify(converter => converter.Convert(dataProperties), Times.Once);
            Assert.That(messageReceivedEventCalled, Is.True);
            Assert.That(messageWithMetadataReceivedCalled, Is.True);
        }


        [Test]
        public void Stop_OnStopAfterConsumerStarted_ShouldDisposeConsumingTask()
        {
            // Arrange
            SetUpConcreteConsumer();
            _netqRabbitConsumer.MessageReceived += x => { };
            var cancelTokenMock = new Mock<IDisposable>();
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()))
                .Returns(cancelTokenMock.Object);
            _netqRabbitConsumer.Start();

            // Act
            _netqRabbitConsumer.Stop();

            // Assert
            cancelTokenMock.Verify(token => token.Dispose(), Times.Once);
        }

        [Test]
        public void Stop_OnStopBeforeConsumerStarted_ShouldDoNothing()
        {
            // Arrange
            SetUpConcreteConsumer();
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()));

            // Act
            _netqRabbitConsumer.Stop();

            // Assert
            _netqAdvancedBusMock.Verify(bus =>
                bus.Consume(It.IsAny<IQueue>(), It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()),
                Times.Never);
        }
    }
}
