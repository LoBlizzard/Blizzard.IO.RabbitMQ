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
            _converterMock = new Mock<IConverter<MessageProperties, RabbitMessageProperties>>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger>();

            _sourceQueue = new RabbitQueue { Name = "EXAMPLE", Exclusive = false};

            _loggerFactoryMock.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
            _netqBusMock.SetupGet(bus => bus.Advanced).Returns(_netqAdvancedBusMock.Object);

            _netqRabbitConsumer = new NetqRabbitConsumer<int>(_netqBusMock.Object, _sourceQueue,
                _deserializerMock.Object, _loggerFactoryMock.Object, _converterMock.Object);
        }

        [Test]
        public void Start_OnStartConsuming_ShouldTriggerConsumeOnce()
        {
            // Arrange
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
        public void Stop_OnStopAfterConsumerStarted_ShouldDisposeConsumingTask()
        {
            // Arrange
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
