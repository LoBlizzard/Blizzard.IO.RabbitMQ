using Blizzard.IO.Core.Rpc;
using Microsoft.Extensions.Logging;
using Blizzard.IO.RabbitMQ.Rpc;
using Blizzard.IO.Serialization.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;

namespace Blizzard.IO.RabbitMQ.Builders.Rpc
{
    public class NetqRabbitRpcClientBuilder : BaseNetqRabbitRpcBuilder
    {
        private ISerializer _serializer = new JsonSerializer();
        private string _hostname = "localhost";
        private string _password = "guest";
        private string _username = "guest";
        private RpcConfiguration _rpcConfiguration = new RpcConfiguration 
        {
            ExchangeNameProvider = type=>"RPC_EXCHANGE",
            RoutingKeyProvider = type=>type.ToString()
        };

        public NetqRabbitRpcClientBuilder(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public IRpcClient Build()
        {
            var connection = InitConnection(_hostname, _password, _username, _rpcConfiguration, _serializer);

            return new NetqRabbitRpcClient()
        }
    }
}
