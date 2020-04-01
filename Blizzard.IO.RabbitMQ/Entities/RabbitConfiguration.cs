using System.Collections.Generic;

namespace Blizzard.IO.RabbitMQ.Entities
{
    public class RabbitConfiguration
    {
        public List<RabbitExchange> Exchanges { get; set; }
        public List<RabbitQueue> Queues { get; set; }
        public List<RabbitBinding> ExchangeToQueueBindings { get; set; }
        public List<RabbitBinding> ExchangeToExchangeBindings { get; set; }
    }
}
