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
        private readonly IDictionary<string, IExchange> _nameToNetqExchange;
        private readonly IDictionary<string, IQueue> _nameToNetqQueue;
        private readonly ILogger<NetqRabbitConfigurator> _logger;

        public NetqRabbitConfigurator(IBus netqBus, ILogger<NetqRabbitConfigurator> logger)
        {
            _netqBus = netqBus ?? throw new ArgumentNullException(nameof(netqBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _nameToNetqExchange = new Dictionary<string, IExchange>();
            _nameToNetqQueue = new Dictionary<string, IQueue>();
        }

        public void Configure(RabbitConfiguration configuration)
        {
            _nameToNetqExchange.Clear();
            _nameToNetqQueue.Clear();
            _logger.LogInformation("Cleared all previous declarations");

            DeclareExchanges(configuration.Exchanges);
            DeclareQueues(configuration.Queues);
            DeclareExchangeToQueueBindings(configuration.ExchangeToQueueBindings);
            DeclareExchangeToExchangeBindings(configuration.ExchangeToExchangeBindings);
            _logger.LogInformation("Declared topology succesfully on bus");
        }

        private void DeclareExchangeToExchangeBindings(List<RabbitBinding> exchangeToExchangeBindings)
        {
            foreach (RabbitBinding binding in exchangeToExchangeBindings)
            {
                _netqBus.Advanced.Bind(
                    _nameToNetqExchange[binding.SourceName],
                    _nameToNetqExchange[binding.DestName],
                    binding.RoutingKey
                    );
            }

            _logger.LogDebug("Declared all exchange to exchange bindings");
        }

        private void DeclareExchangeToQueueBindings(List<RabbitBinding> exchangeToQueueBinding)
        {
            foreach (RabbitBinding binding in exchangeToQueueBinding)
            {
                _netqBus.Advanced.Bind(
                    _nameToNetqExchange[binding.SourceName],
                    _nameToNetqQueue[binding.DestName],
                    binding.RoutingKey
                    );
            }

            _logger.LogDebug("Declared all exchange to queue bindings");
        }

        private void DeclareQueues(List<RabbitQueue> queues)
        {
            foreach (RabbitQueue queue in queues)
            {
                _nameToNetqQueue[queue.Name] = _netqBus.Advanced.QueueDeclare(
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
        }

        private void DeclareExchanges(List<RabbitExchange> exchanges)
        {
            foreach (RabbitExchange exchange in exchanges)
            {
                _nameToNetqExchange[exchange.Name] = _netqBus.Advanced.ExchangeDeclare(
                    exchange.Name,
                    Helpers.ExchangeTypeToStringResolver[exchange.Type],
                    exchange.Passive,
                    exchange.Durable,
                    exchange.AutoDelete,
                    exchange.Internal,
                    exchange.AlternateExchange,
                    exchange.Delayed
                    );

                _logger.LogDebug($"Declared exchange: {exchange.Name} succesfully");
            }
        }
    }
}
