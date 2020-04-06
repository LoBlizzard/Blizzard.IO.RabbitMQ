using Blizzard.IO.Core.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blizzard.IO.RabbitMQ.Builders.Rpc
{
    public class NetqRabbitRpcClientBuilder : BaseNetqRabbitRpcBuilder
    {
        public NetqRabbitRpcClientBuilder(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public IRpcClient Build()
        {

        }
    }
}
