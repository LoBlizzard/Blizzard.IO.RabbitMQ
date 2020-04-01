using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;
using System;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public interface INetqRpcRabbitConnection<TReturnType>
    {
        IBus RabbitBus { get; }
        RpcMessageType MessageType { get; }
    }
}
