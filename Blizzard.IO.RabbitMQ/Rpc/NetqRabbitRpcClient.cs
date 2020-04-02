using Blizzard.IO.Core.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcClient : IRpcClient
    {
        private readonly INetqRabbitRpcConnection<Func<Type, object>> _netqRpcRabbitConnection;
        private readonly ILogger _logger;

        public NetqRabbitRpcClient(INetqRabbitRpcConnection<Func<Type, object>> netqRpcRabbitConnection, ILogger logger)
        {
            _logger = logger;
            _netqRpcRabbitConnection = netqRpcRabbitConnection;
        }

        public TRespond Request<TRequest, TRespond>(TRequest request)
        where TRequest : class
        where TRespond : class
        {
            if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _logger.LogDebug($"Requesting an abstract type request of type: {typeof(TRequest)} and expecting to receive a respond of type: {typeof(TRespond)}");
                return _netqRpcRabbitConnection.Bus.Request<TRequest, TRespond>(request);
            }
            else if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _logger.LogDebug($"Requesting a concrete type request of type: {typeof(TRequest)} and expecting to receive a respond of type: {typeof(TRespond)}");
                var func = _netqRpcRabbitConnection.Bus.Request<TRequest, Func<Type, object>>(request);
                return (TRespond)func(typeof(TRespond));
            }

            throw new InvalidOperationException($"Cannot handle request {nameof(_netqRpcRabbitConnection)} rpc type is invalid, " +
                $"rpc type: {_netqRpcRabbitConnection.RpcMessageType}");
        }

        public async Task<TRespond> RequestASync<TRequest, TRespond>(TRequest request)
        where TRequest : class
        where TRespond : class
        {
            if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _logger.LogDebug($"Requesting an abstract type async request of type: {typeof(TRequest)} and expecting to receive a respond of type: {typeof(TRespond)}");
                return await _netqRpcRabbitConnection.Bus.RequestAsync<TRequest, TRespond>(request);
            }
            else if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _logger.LogDebug($"Requesting a concrete type async request of type: {typeof(TRequest)} and expecting to receive a respond of type: {typeof(TRespond)}");
                Func<Type, object> func = await _netqRpcRabbitConnection.Bus.RequestAsync<TRequest, Func<Type, object>>(request);
                return (TRespond)func(typeof(TRespond));
            }

            throw new InvalidOperationException($"Cannot handle request {nameof(_netqRpcRabbitConnection)} rpc type is invalid, " +
                $"rpc type: {_netqRpcRabbitConnection.RpcMessageType}");
        }
    }
}
