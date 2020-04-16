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
            if (configuration.RequestExchangeNameProvider != null)
            {
                RpcExchangeNameConvention requestExchangeNameProvider = type => configuration.RequestExchangeNameProvider(type);
                conventions.RpcRequestExchangeNamingConvention = requestExchangeNameProvider;
            }
            if (configuration.RoutingKeyProvider!=null)
            {
                RpcRoutingKeyNamingConvention routingKeyProvider = type => configuration.RoutingKeyProvider(type);
                conventions.RpcRoutingKeyNamingConvention = routingKeyProvider;
            }
            if (configuration.ResponseExchangeNameProvider != null)
            {
                RpcExchangeNameConvention responseExchangeNameProvider = type => configuration.ResponseExchangeNameProvider(type);
                conventions.RpcResponseExchangeNamingConvention = responseExchangeNameProvider;
            }
            if (configuration.ReturnQueueNameProvider!=null)
            {
                RpcReturnQueueNamingConvention returnQueueNamingConvention = () => configuration.ReturnQueueNameProvider();
                conventions.RpcReturnQueueNamingConvention = returnQueueNamingConvention;
            }
        }
    }
}
