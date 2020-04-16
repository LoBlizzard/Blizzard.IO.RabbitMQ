using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public interface INetqRabbitRpcConnection : IDisposable
    {
        IBus Bus { get; }
        RpcMessageType RpcMessageType { get; }

        Func<Type, object> Request<TRequest>(TRequest request)
            where TRequest : class;
        Task<Func<Type, object>> RequestAsync<TRequest>(TRequest request)
            where TRequest : class;
        IDisposable Respond<TRequest, TRespond>(Func<Func<Type, object>, TRespond> callback)
            where TRespond : class
            where TRequest : class;
        IDisposable RespondAsync<TRequest, TRespond>(Func<Func<Type, object>, Task<TRespond>> callback)
            where TRespond : class
            where TRequest : class;
    }
}
