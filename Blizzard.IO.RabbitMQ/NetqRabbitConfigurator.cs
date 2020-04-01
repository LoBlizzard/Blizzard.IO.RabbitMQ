using System;
using System.Collections.Generic;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;

namespace Blizzard.IO.RabbitMQ
{
    public class NetqRabbitConfigurator : IRabbitConfigurator
    {
        private readonly IBus _netqBus;

        public NetqRabbitConfigurator(IBus netqBus)
        {
            _netqBus = netqBus ?? throw new ArgumentNullException(nameof(netqBus));
        }

        public void Configure(RabbitConfiguration configuration)
        {
            DeclareExchanges(configuration.Exchanges);
            DeclareQueues(configuration.Queues);
        }

        private void DeclareExchanges(List<RabbitExchange> exchanges)
        {
            foreach (RabbitExchange exchange in exchanges)
            {
                _netqBus.Advanced.ExchangeDeclare(
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
