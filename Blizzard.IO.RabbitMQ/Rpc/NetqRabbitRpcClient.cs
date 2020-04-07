using Blizzard.IO.Core.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcClient : IRpcClient
    {
        private readonly INetqRabbitRpcConnection _netqRpcRabbitConnection;
        private readonly ILogger _logger;

        public NetqRabbitRpcClient(INetqRabbitRpcConnection netqRpcRabbitConnection, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(nameof(NetqRabbitRpcClient));
            _netqRpcRabbitConnection = netqRpcRabbitConnection;
        }

        public TRespond Request<TRequest, TRespond>(TRequest request)
        where TRequest : class
        where TRespond : class
        {
            _logger.LogDebug($"Requesting an {_netqRpcRabbitConnection.RpcMessageType} message type request of type: {typeof(TRequest)} and expecting to receive a respond of type: {typeof(TRespond)}");

            if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                return _netqRpcRabbitConnection.Bus.Request<TRequest, TRespond>(request);
            }
            else if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                Func<Type, object> func = _netqRpcRabbitConnection.Request(request);
                return (TRespond)func(typeof(TRespond));
            }

            throw new InvalidOperationException($"Cannot handle request {nameof(_netqRpcRabbitConnection)} rpc type is invalid, " +
                $"rpc type: {_netqRpcRabbitConnection.RpcMessageType}");
        }

        public async Task<TRespond> RequestAsync<TRequest, TRespond>(TRequest request)
        where TRequest : class
        where TRespond : class
        {
            _logger.LogDebug($"Requesting an {_netqRpcRabbitConnection.RpcMessageType} message type async request of type: {typeof(TRequest)} and expecting to receive a respond of type: {typeof(TRespond)}");

            if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                return await _netqRpcRabbitConnection.Bus.RequestAsync<TRequest, TRespond>(request);
            }
            else if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                Func<Type, object> func = await _netqRpcRabbitConnection.RequestAsync(request);
                return (TRespond)func(typeof(TRespond));
            }

            throw new InvalidOperationException($"Cannot handle request {nameof(_netqRpcRabbitConnection)} rpc type is invalid, " +
                $"rpc type: {_netqRpcRabbitConnection.RpcMessageType}");
        }
    }
}
