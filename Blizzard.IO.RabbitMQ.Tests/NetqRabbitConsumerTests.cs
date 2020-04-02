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
        private Mock<IConcreteTypeDeserializer<int>> _abstractTypeDeserializerMock;
        private Mock<IQueue> _sourceQueueMock;
        private Mock<IConverter<MessageProperties, RabbitMessageProperties>> _converterMock;
        private ILoggerFactory _loggerFactory;

        private NetqRabbitConsumer<int> _netqRabbitConsumer;

        [SetUp]
        public void SetUp()
        {
            _netqBusMock = new Mock<IBus>();
            _netqAdvancedBusMock = new Mock<IAdvancedBus>();
            _deserializerMock = new Mock<IDeserializer<int>>();
            _abstractTypeDeserializerMock = new Mock<IConcreteTypeDeserializer<int>>();
            _sourceQueueMock = new Mock<IQueue>();
            _converterMock = new Mock<IConverter<MessageProperties, RabbitMessageProperties>>();
            _loggerFactory = new LoggerFactory();

            _netqBusMock.SetupGet(bus => bus.Advanced).Returns(_netqAdvancedBusMock.Object);

            _netqRabbitConsumer = new NetqRabbitConsumer<int>(_netqBusMock.Object, _sourceQueueMock.Object,
                _deserializerMock.Object, _loggerFactory, _converterMock.Object);
        }

        [Test]
        public void Start_OnStartConsuming_ShouldStartConsumingFromSourceQueue()
        {
            // Arrange
            _netqAdvancedBusMock.Setup(bus => bus.Consume(It.IsAny<IQueue>(),
                It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()));

            // Act
            _netqRabbitConsumer.Start();

            // Assert
            _netqAdvancedBusMock.Verify(bus =>
                bus.Consume(_sourceQueueMock.Object, It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()),
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

            // Act
            _netqRabbitConsumer.Start();
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
                bus.Consume(_sourceQueueMock.Object, It.IsAny<Action<byte[], MessageProperties, MessageReceivedInfo>>()),
                Times.Never);
        }
    }
}
