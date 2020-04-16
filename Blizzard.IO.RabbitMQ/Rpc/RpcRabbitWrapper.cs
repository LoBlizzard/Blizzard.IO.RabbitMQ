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
                conventions.RpcRequestExchangeNamingConvention = type => configuration.RequestExchangeNameProvider(type);
            }
            if (configuration.RoutingKeyProvider!=null)
            {
                conventions.RpcRoutingKeyNamingConvention = type => configuration.RoutingKeyProvider(type);
            }
            if (configuration.ResponseExchangeNameProvider != null)
            {
                conventions.RpcResponseExchangeNamingConvention = type => configuration.ResponseExchangeNameProvider(type);
            }
            if (configuration.ReturnQueueNameProvider!=null)
            {
                conventions.RpcReturnQueueNamingConvention = () => configuration.ReturnQueueNameProvider();
            }
        }
    }
}
