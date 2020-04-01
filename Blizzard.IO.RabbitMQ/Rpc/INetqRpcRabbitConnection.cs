using EasyNetQ;
using System;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public interface INetqRpcRabbitConnection
    {
        IBus RabbitBus { get; }
        RpcMessageType MessageType { get; }

        T GetObject<T>(byte[] data, Type type);
    }
}
