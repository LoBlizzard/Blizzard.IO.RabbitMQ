using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
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
        private NetqRabbitConfigurator _netqRabbitConfigurator;
        private Mock<IBus> _netqBusMock;
        private Mock<IAdvancedBus> _netqAdvancedBusMock;
        private Mock<ILogger<NetqRabbitConfigurator>> _loggerMock;

        [SetUp]
        public void SetUp()
        {
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
            Assert.That(() => _netqRabbitConfigurator.Configure(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Configure_OnRabbitExchangeFoundConfigurationExchanges_ShouldDeclareThatExchangePropertiesOnBus()
        {
            // Arrange
            var exchange = new RabbitExchange
            {
                Name = "0",
                Type = RabbitExchangeType.Direct,
                Passive = false,
                Durable = false,
                AutoDelete = false,
                Internal = false,
                AlternateExchange = "2",
                Delayed = false
            };
            _configuration.Exchanges.Add(exchange);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus => bus.ExchangeDeclare(
                exchange.Name,
                Helpers.ExchangeTypeToStringResolver[exchange.Type],
                exchange.Passive,
                exchange.Durable,
                exchange.AutoDelete,
                exchange.Internal,
                exchange.AlternateExchange,
                exchange.Delayed), Times.Once);
        }

        [Test]
        public void Configure_OnRabbitQueue_ShouldDeclareThatExchangesPropertiesOnBus()
        {
            // Arrange
            var queue = new RabbitQueue
            {
                Name = "0",
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
            _configuration.Queues.Add(queue);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus => bus.QueueDeclare(
                queue.Name,
                queue.Passive,
                queue.Durable,
                queue.Exclusive,
                queue.AutoDelete,
                queue.PerQueueMessageTtl,
                queue.Expires,
                queue.MaxPriority,
                queue.DeadLetterExchange,
                queue.DeadLetterRoutingKey,
                queue.MaxLength,
                queue.MaxLengthBytes), Times.Once);
        }

        [Test]
        public void Configure_OnExchangeToExchangeBindingFound_ShouldBindThatRelation()
        {
            // Arrange
            var binding = new RabbitBinding
            {
                SourceName = "source",
                DestName = "dest",
                RoutingKey = "/"
            };
            _configuration.ExchangeToExchangeBindings.Add(binding);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus => 
                bus.Bind(binding.SourceName, binding.DestName, binding.RoutingKey), Times.Once);
        }

        [Test]
        public void Configure_OnExchangeToQueueBindingFound_ShouldBindThatRelation()
        {
            // Arrange
            var binding = new RabbitBinding
            {
                SourceName = "source",
                DestName = "dest",
                RoutingKey = "/"
            };
            _configuration.ExchangeToQueueBindings.Add(binding);

            // Act
            _netqRabbitConfigurator.Configure(_configuration);

            // Assert
            _netqAdvancedBusMock.Verify(bus =>
                bus.Bind(binding.SourceName, binding.DestName, binding.RoutingKey), Times.Once);
        }
    }
}
    