using System;

namespace Blizzard.IO.RabbitMQ.Entities.Rpc
{
    public class RpcConfiguration
    {
        public Func<Type, string> RequestExchangeNameProvider { get; set; }
        public Func<Type, string> RoutingKeyProvider { get; set; }
        public Func<Type, string> ResponseExchangeNameProvider { get; set; }
        public Func<string> ReturnQueueNameProvider { get; set; }
    }
}
