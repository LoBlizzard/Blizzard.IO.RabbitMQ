using System;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class RpcConfiguration
    {
        // Im not sure if to geive the user a function or to give a static string to return
        public readonly Func<Type, string> ExchangeNameProvider;
        public readonly Func<Type, string> RoutingKeyProvider;
    }
}
