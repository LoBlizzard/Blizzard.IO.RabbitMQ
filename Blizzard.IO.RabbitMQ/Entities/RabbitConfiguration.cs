using System.Collections.Generic;

namespace Blizzard.IO.RabbitMQ.Entities
{
    public class RabbitConfiguration
    {
        public List<RabbitExchange> Exchanges { get; set; }
        public List<RabbitQueue> Queues { get; set; }
        public List<(string, string, string)> ExchangeToQueueBindings { get; set; }
        public List<(string, string, string)> ExchangeToExchangeBindings { get; set; }
    }
}
