using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Blizzard.IO.RabbitMQ.Tests
{
    [TestFixture]
    public class NetqRabbitConfiguratorTests
    {
        private RabbitConfiguration _configuration;
        private RabbitExchange _exchange;
        private RabbitQueue _queue;

        private Mock<IBus> _netqBusMock;
        private Mock<IAdvancedBus> _netqAdvancedBusMock;
        private Mock<ILogger<NetqRabbitConfigurator>> _loggerMock;
        private NetqRabbitConfigurator _netqRabbitConfigurator;

        [SetUp]
        public void SetUp()
        {
            _exchange = new RabbitExchange
            {
                Name = "EXCHANGE",
                Type = RabbitExchangeType.Direct,
                Passive = false,
                Durable = false,
                AutoDelete = false,
                Internal = false,
                AlternateExchange = "2",
                Delayed = false
            };

            _queue = new RabbitQueue
            {
                Name = "QUEUE",
                Passive = false,
                Durable = false,
                Exclusive = false,
                AutoDelete = false,
                PerQueueMessageTtl = 12,
                Expires = 3,
                MaxPriority = 4,
                DeadLetterExchange = "are",
                DeadLetterRoutingKey = "really...",
                MaxLength = 10,
                MaxLengthBytes = 1
            };

            _configuration = new RabbitConfiguration
            {
                Exchanges = new List<RabbitExchange>(),
                Queues = new List<RabbitQueue>(),
                ExchangeToExchangeBindings = new List<RabbitBinding>(),
                ExchangeToQueueBindings = new List<RabbitBinding>()
            };

            _loggerMock = new Mock<ILogger<NetqRabbitConfigurator>>();
            _netqAdvancedBusMock = new Mock<IAdvancedBus>();
            _netqBusMock = new Mock<IBus>();
            _netqBusMock.SetupGet(bus => bus.Advanced).Returns(_netqAdvancedBusMock.Object);
            _netqRabbitConfigurator = new NetqRabbitConfigurator(_netqBusMock.Object, _loggerMock.Object);
        }

        [Test]
        public void Configure_OnRabbitConfigurationNull_ShouldThrowNullArgumentException()
        {
            // Act + Assert
            Assert.That(() => _netqRabbitConfigurator.Configure(null), Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void Configure_OnRabbitExchangeFoundConfigurationExchanges_ShouldDeclareThatExchangePropertiesOnBus()
        {
            // Arrange
            _configuration.Exchanges.Add(_exchange);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus => bus.ExchangeDeclare(
                _exchange.Name,
                Utilities.ExchangeTypeToStringResolver[_exchange.Type],
                _exchange.Passive,
                _exchange.Durable,
                _exchange.AutoDelete,
                _exchange.Internal,
                _exchange.AlternateExchange,
                _exchange.Delayed), Times.Once);
        }

        [Test]
        public void Configure_OnRabbitQueue_ShouldDeclareThatExchangesPropertiesOnBus()
        {
            // Arrange
            _configuration.Queues.Add(_queue);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus => bus.QueueDeclare(
                _queue.Name,
                _queue.Passive,
                _queue.Durable,
                _queue.Exclusive,
                _queue.AutoDelete,
                _queue.PerQueueMessageTtl,
                _queue.Expires,
                _queue.MaxPriority,
                _queue.DeadLetterExchange,
                _queue.DeadLetterRoutingKey,
                _queue.MaxLength,
                _queue.MaxLengthBytes), Times.Once);
        }

        [Test]
        public void Configure_OnExchangeToExchangeBindingFound_ShouldBindThatRelation()
        {
            // Arrange
            var binding = new RabbitBinding
            {
                SourceName = "EXCHANGE",
                DestName = "EXCHANGE",
                RoutingKey = "/"
            };

            _configuration.Exchanges.Add(_exchange);
            _configuration.Exchanges.Add(_exchange);
            _configuration.ExchangeToExchangeBindings.Add(binding);

            var netqExchange = new Exchange("EXCHANGE");
            _netqAdvancedBusMock.Setup(bus => bus.ExchangeDeclare(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(),It.IsAny<string>(), It.IsAny<bool>())).Returns(netqExchange);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus => 
                bus.Bind(netqExchange, netqExchange, binding.RoutingKey), Times.Once);
        }

        [Test]
        public void Configure_OnExchangeToQueueBindingFound_ShouldBindThatRelation()
        {
            // Arrange
            var binding = new RabbitBinding
            {
                SourceName = "EXCHANGE",
                DestName = "QUEUE",
                RoutingKey = "/"
            };

            _configuration.Exchanges.Add(_exchange);
            _configuration.Queues.Add(_queue);
            _configuration.ExchangeToQueueBindings.Add(binding);

            var netqExchange = new Exchange("EXCHANGE");
            _netqAdvancedBusMock.Setup(bus => bus.ExchangeDeclare(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(netqExchange);

           var netqQueue = new Queue("QUEUE", false);
            _netqAdvancedBusMock.Setup(bus => bus.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(netqQueue);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus =>
                bus.Bind(netqExchange, netqQueue, binding.RoutingKey), Times.Once);
        }
    }
}
    