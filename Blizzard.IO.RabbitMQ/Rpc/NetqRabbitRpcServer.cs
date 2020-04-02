using Blizzard.IO.Core.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcServer : IRpcServer
    {
        private readonly INetqRpcRabbitConnection<Func<Type,object>> _netqRabbitRpcConnection;
        private readonly ILogger _logger;

        private IDisposable _respondHandler;

        public void Respond<TRequest, TRespond>(Func<TRequest, TRespond> callback)
            where TRequest : class
            where TRespond : class
        {
            if (_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _respondHandler = _netqRabbitRpcConnection.Bus.Respond<TRequest, TRespond>(callback);
            }
            else if(_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _respondHandler = _netqRabbitRpcConnection.Bus.Respond<Func<Type,object>, TRespond>(func =>
                 {
                     TRequest request = (TRequest)func(typeof(TRequest));
                     return callback(request);
                 });
            }

            throw new InvalidOperationException($"Cannot handle request {nameof(_netqRabbitRpcConnection)} rpc type is invalid, " +
                $"rpc type: {_netqRabbitRpcConnection.RpcMessageType}");
        }

        public void RespondAsync<TRequest, TRespond>(Func<TRequest, Task<TRespond>> callback)
            where TRequest : class
            where TRespond : class
        {
            if (_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _respondHandler = _netqRabbitRpcConnection.Bus.RespondAsync<TRequest, TRespond>(callback);
            }
            else if (_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _respondHandler = _netqRabbitRpcConnection.Bus.RespondAsync<Func<Type, object>, TRespond>(func =>
                {
                    TRequest request = (TRequest)func(typeof(TRequest));
                    return callback(request);
                });
            }

            throw new InvalidOperationException($"Cannot handle request {nameof(_netqRabbitRpcConnection)} rpc type is invalid, " +
                $"rpc type: {_netqRabbitRpcConnection.RpcMessageType}");
        }
    }
}
