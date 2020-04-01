using System;
using System.Collections.Generic;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Blizzard.IO.RabbitMQ
{
    public class NetqRabbitConfigurator : IRabbitConfigurator
    {
        private readonly IBus _netqBus;
        private readonly IDictionary<string, IExchange> _nameToNetqExchange;
        private readonly IDictionary<string, IQueue> _nameToNetqQueue;

        public NetqRabbitConfigurator(IBus netqBus)
        {
            _netqBus = netqBus ?? throw new ArgumentNullException(nameof(netqBus));
            _nameToNetqExchange = new Dictionary<string, IExchange>();
            _nameToNetqQueue = new Dictionary<string, IQueue>();
        }

        public void Configure(RabbitConfiguration configuration)
        {
            _nameToNetqExchange.Clear();
            _nameToNetqQueue.Clear(); 
            DeclareExchanges(configuration.Exchanges);
            DeclareQueues(configuration.Queues);
            DeclareExchangesToQueuesBindings(configuration.ExchangeToQueueBindings);
        }

        private void DeclareExchangesToQueuesBindings(List<(RabbitExchange, RabbitQueue, string)> exchangeToQueueBindings)
        {
           
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
            }
        }
    }
}
