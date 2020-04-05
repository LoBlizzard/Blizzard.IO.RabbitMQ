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
        private readonly ILogger _logger;

        public NetqRabbitConfigurator(IBus netqBus, ILoggerFactory loggerFactory)
        {
            _netqBus = netqBus;
            _logger = loggerFactory.CreateLogger(nameof(NetqRabbitConfigurator));
        }

        public void Configure(RabbitConfiguration configuration)
        {
            IDictionary<string, IExchange> nameToNetqExchange = DeclareExchanges(configuration.Exchanges);
            IDictionary<string, IQueue> nameToNetqQueue = DeclareQueues(configuration.Queues);

            DeclareExchangeToExchangeBindings(configuration.ExchangeToExchangeBindings, nameToNetqExchange);
            DeclareExchangeToQueueBindings(configuration.ExchangeToQueueBindings, nameToNetqExchange, nameToNetqQueue);

            _logger.LogInformation("Declared topology succesfully on bus");
        }

        private void DeclareExchangeToQueueBindings(IReadOnlyCollection<RabbitBinding> bindings,
            IDictionary<string, IExchange> nameToNetqExchange, IDictionary<string, IQueue> nameToNetqQueue)
        {
            foreach (RabbitBinding binding in bindings)
            {
                _netqBus.Advanced.Bind(
                    nameToNetqExchange[binding.SourceName],
                    nameToNetqQueue[binding.DestName],
                    binding.RoutingKey
                    );
                _logger.LogDebug($"Bind exchange: {binding.SourceName} to queue: {binding.DestName}");
            }
        }

        private void DeclareExchangeToExchangeBindings(IReadOnlyCollection<RabbitBinding> bindings,
            IDictionary<string, IExchange> nameToNetqExchange)
        {
            foreach (RabbitBinding binding in bindings)
            {
                _netqBus.Advanced.Bind(
                    nameToNetqExchange[binding.SourceName],
                    nameToNetqExchange[binding.DestName],
                    binding.RoutingKey
                    );
                _logger.LogDebug($"Bind exchange: {binding.SourceName} to exchange: {binding.DestName}");
            }
        }

        private IDictionary<string, IQueue> DeclareQueues(IReadOnlyCollection<RabbitQueue> queues)
        {
            var nameToNetqQueue = new Dictionary<string, IQueue>();
            foreach (RabbitQueue queue in queues)
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

            return nameToNetqQueue;
        }

        private IDictionary<string, IExchange> DeclareExchanges(IReadOnlyCollection<RabbitExchange> exchanges)
        {
            var nameToNetqExchange = new Dictionary<string, IExchange>();
            foreach (RabbitExchange exchange in exchanges)
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

            return nameToNetqExchange;
        }
    }
}
