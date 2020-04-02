using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public interface INetqRpcRabbitConnection<TReturnType>
    {
        IBus Bus { get; }
        RpcMessageType RpcMessageType { get; }
    }
}
