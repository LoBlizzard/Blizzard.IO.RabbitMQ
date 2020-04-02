using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public interface INetqRabbitRpcConnection<TReturnType>
    {
        IBus Bus { get; }
        RpcMessageType RpcMessageType { get; }
    }
}
