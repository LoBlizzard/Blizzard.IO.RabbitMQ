using System;
using System.Collections.Generic;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ
{
    public class NetqRabbitConfigurator : IRabbitConfigurator
    {
        private readonly IBus _netqBus;
        private readonly ILogger<NetqRabbitConfigurator> _logger;

        public NetqRabbitConfigurator(IBus netqBus, ILogger<NetqRabbitConfigurator> logger)
        {
            _netqBus = netqBus ?? throw new ArgumentNullException(nameof(netqBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Configure(RabbitConfiguration configuration)
        {
            var nameToNetqExchange = new Dictionary<string, IExchange>();
            var nameToNetqQueue = new Dictionary<string, IQueue>();

            foreach (RabbitExchange exchange in configuration.Exchanges)
            {
                nameToNetqExchange[exchange.Name] = _netqBus.Advanced.ExchangeDeclare(
                    exchange.Name,
                    Utilities.ExchangeTypeToStringResolver[exchange.Type],
                    exchange.Passive,
                    exchange.Durable,
                    exchange.AutoDelete,
                    exchange.Internal,
                    exchange.AlternateExchange,
                    exchange.Delayed
                    );

                _logger.LogDebug($"Declared exchange: {exchange.Name} succesfully");
            }

            foreach (RabbitQueue queue in configuration.Queues)
            {
                nameToNetqQueue[queue.Name] = _netqBus.Advanced.QueueDeclare(
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
                    queue.MaxLengthBytes
                    );

                _logger.LogDebug($"Declared queue: {queue.Name} succesfully");
            }

            foreach (RabbitBinding binding in configuration.ExchangeToQueueBindings)
            {
                _netqBus.Advanced.Bind(
                    nameToNetqExchange[binding.SourceName],
                    nameToNetqQueue[binding.DestName],
                    binding.RoutingKey
                    );
            }

            _logger.LogDebug("Declared all exchange to queue bindings");

            foreach (RabbitBinding binding in configuration.ExchangeToExchangeBindings)
            {
                _netqBus.Advanced.Bind(
                    nameToNetqExchange[binding.SourceName],
                    nameToNetqExchange[binding.DestName],
                    binding.RoutingKey
                    );
            }

            _logger.LogDebug("Declared all exchange to exchange bindings");

            nameToNetqExchange.Clear();
            nameToNetqQueue.Clear();
            _logger.LogDebug("Cleared all previous declarations");

            _logger.LogInformation("Declared topology succesfully on bus");
        }
    }
}
