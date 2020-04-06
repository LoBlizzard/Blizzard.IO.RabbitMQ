using Blizzard.IO.Core.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcServer : IRpcServer
    {
        private readonly INetqRabbitRpcConnection<Func<Type,object>> _netqRpcRabbitConnection;
        private IDisposable _respondHandler;
        private bool _isStarted;
        private readonly ILogger _logger;

        public NetqRabbitRpcServer(INetqRabbitRpcConnection<Func<Type,object>> netqRpcRabbitConnection, ILoggerFactory loggerFactory)
        {
            _netqRpcRabbitConnection = netqRpcRabbitConnection;
            _respondHandler = null;
            _isStarted = false;
            _logger = loggerFactory.CreateLogger(nameof(NetqRabbitRpcServer));
        }

        public void Respond<TRequest, TRespond>(Func<TRequest, TRespond> callback)
            where TRequest : class
            where TRespond : class
        {
            VerifyServerHasntStartedYet();
            if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _respondHandler = _netqRpcRabbitConnection.Bus.Respond<TRequest, TRespond>(callback);
                _logger.LogInformation($"Started responding on bus: {_netqRpcRabbitConnection.Bus}, of type {_netqRpcRabbitConnection.RpcMessageType}");
            }
            else if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _respondHandler = _netqRpcRabbitConnection.Bus.Respond<Func<Type, object>, TRespond>(func =>
                  {
                      TRequest request = (TRequest)func(typeof(TRequest));
                      return callback(request);
                  });
            }
            else
            {
                throw new InvalidOperationException($"Cannot start responding rpc type is invalid, rpc type: {_netqRpcRabbitConnection.RpcMessageType}");
            }

            _isStarted = true;
            _logger.LogInformation($"Started responding on bus: {_netqRpcRabbitConnection.Bus}, of type {_netqRpcRabbitConnection.RpcMessageType}");
        }

        public void RespondAsync<TRequest, TRespond>(Func<TRequest, Task<TRespond>> callback)
            where TRequest : class
            where TRespond : class
        {
            VerifyServerHasntStartedYet();
            if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _respondHandler = _netqRpcRabbitConnection.Bus.RespondAsync<TRequest, TRespond>(callback);
            }
            else if (_netqRpcRabbitConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _respondHandler = _netqRpcRabbitConnection.Bus.RespondAsync<Func<Type, object>, TRespond>(func =>
                {
                    TRequest request = (TRequest)func(typeof(TRequest));
                    return callback(request);
                });
            }
            else
            {
                throw new InvalidOperationException($"Cannot start responding rpc type is invalid, rpc type: {_netqRpcRabbitConnection.RpcMessageType}");
            }

            _isStarted = true;
            _logger.LogInformation($"Started responding async on bus: {_netqRpcRabbitConnection.Bus}, of type {_netqRpcRabbitConnection.RpcMessageType}");
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _respondHandler.Dispose();
                _respondHandler = null;
                _isStarted = false;
                _logger.LogInformation($"Stoped responding on bus: {_netqRpcRabbitConnection.Bus}, of type {_netqRpcRabbitConnection.RpcMessageType}");
            }
            else
            {
                _logger.LogWarning("Could not stop responding cause this server hasnt started yet or has already been stopped");
            }
        }

        private void VerifyServerHasntStartedYet()
        {
            if (_isStarted)
            {
                throw new InvalidOperationException($"Error this server already responding");
            }
        }
    }
}
