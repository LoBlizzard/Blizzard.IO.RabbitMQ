using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public interface INetqRpcRabbitConnection<T>
        where T : class
    {
        IBus Bus { get; }
        RpcMessageType RpcMessageType { get; }

        T Request<TRequest>(TRequest request)
            where TRequest : class;
        Task<T> RequestAsync<TRequest>(TRequest request)
            where TRequest : class;
        IDisposable Respond<TRespond>(Func<T, TRespond> callback)
            where TRespond : class;
        IDisposable RespondAsync<TRespond>(Func<T, Task<TRespond>> callback)
            where TRespond : class;
    }
}
