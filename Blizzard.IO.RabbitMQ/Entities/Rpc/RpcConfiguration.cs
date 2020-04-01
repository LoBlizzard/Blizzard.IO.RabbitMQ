using System;

namespace Blizzard.IO.RabbitMQ.Entities.Rpc
{
    public class RpcConfiguration
    {
        // Im not sure if to geive the user a function or to give a static string to return
        public Func<Type, string> ExchangeNameProvider { get; set; }
        public Func<Type, string> RoutingKeyProvider { get; set; }
    }
}
