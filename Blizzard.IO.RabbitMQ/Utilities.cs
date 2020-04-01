using Blizzard.IO.RabbitMQ.Entities;
using System.Collections.Generic;

namespace Blizzard.IO.RabbitMQ
{
    public static class Utilities
    {
        public static Dictionary<RabbitExchangeType, string> ExchangeTypeToStringResolver = new Dictionary<RabbitExchangeType, string>
        {
            [RabbitExchangeType.Fanout] = "fanout",
            [RabbitExchangeType.Direct] = "direct",
            [RabbitExchangeType.Headers] = "headers",
            [RabbitExchangeType.Topic] = "topic",
            [RabbitExchangeType.XConsistentHash] = "x-consistent-hash"
        };
    }
}
