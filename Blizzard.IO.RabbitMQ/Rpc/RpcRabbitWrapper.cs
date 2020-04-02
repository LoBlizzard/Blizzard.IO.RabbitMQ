using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;
using EasyNetQ.Producer;
using EasyNetqRpc = EasyNetQ.Producer.Rpc;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class RpcRabbitWrapper : EasyNetqRpc
    {
        public RpcRabbitWrapper(RpcConfiguration configuration, ConnectionConfiguration connectionConfiguration, IAdvancedBus advancedBus, IEventBus eventBus, IConventions conventions, 
            IExchangeDeclareStrategy exchangeDeclareStrategy, IMessageDeliveryModeStrategy messageDeliveryModeStrategy, ITimeoutStrategy timeoutStrategy, 
            ITypeNameSerializer typeNameSerializer) : base(connectionConfiguration, advancedBus, eventBus, conventions, exchangeDeclareStrategy, 
                messageDeliveryModeStrategy, timeoutStrategy, typeNameSerializer)
        {
            RpcExchangeNameConvention exchangeNameProvider = type=>configuration.ExchangeNameProvider(type);
            RpcRoutingKeyNamingConvention routingKeyProvider = type => configuration.RoutingKeyProvider(type);
            conventions.RpcRequestExchangeNamingConvention = exchangeNameProvider;
            conventions.RpcRoutingKeyNamingConvention = routingKeyProvider;
        }
    }
}
