using Blizzard.IO.Core.Rpc;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcClient : IRpcClient
    {
        private readonly INetqRpcRabbitConnection<Func<Type, object>> _netqRpcRabbitConnection;

        public NetqRabbitRpcClient(INetqRpcRabbitConnection<Func<Type, object>> netqRpcRabbitConnection)
        {
            _netqRpcRabbitConnection = netqRpcRabbitConnection;
        }

        public TRespond Request<TRequest, TRespond>(TRequest request)
        where TRequest : class
        where TRespond : class
        {
            var func = _netqRpcRabbitConnection.RabbitBus.Request<TRequest, Func<Type, object>>(request);
            return (TRespond)func(typeof(TRespond));
        }

        public async Task<TRespond> RequestASync<TRequest, TRespond>(TRequest request)
        where TRequest : class
        where TRespond : class
        {
            Func<Type, object> func = await _netqRpcRabbitConnection.RabbitBus.RequestAsync<TRequest, Func<Type, object>>(request);
            return (TRespond)func(typeof(TRespond));
        }
    }
}
