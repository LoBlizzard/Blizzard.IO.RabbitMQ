using Blizzard.IO.Core.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcServer : IRpcServer
    {
        private readonly INetqRabbitRpcConnection<Func<Type,object>> _netqRabbitRpcConnection;
        private IDisposable _respondHandler;
        private bool _isStarted;
        private readonly ILogger _logger;

        public NetqRabbitRpcServer(INetqRabbitRpcConnection<Func<Type,object>> netqRpcRabbitConnection, ILoggerFactory loggerFactory)
        {
            _netqRabbitRpcConnection = netqRpcRabbitConnection;
            _respondHandler = null;
            _isStarted = false;
            _logger = loggerFactory.CreateLogger(nameof(NetqRabbitRpcServer));
        }

        public void Respond<TRequest, TRespond>(Func<TRequest, TRespond> callback)
            where TRequest : class
            where TRespond : class
        {
            VerifyServerHasntStartedYet();
            if (_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _respondHandler = _netqRabbitRpcConnection.Bus.Respond<TRequest, TRespond>(callback);
            }
            else if (_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _respondHandler = _netqRabbitRpcConnection.Respond(func =>
                  {
                      TRequest request = (TRequest)func(typeof(TRequest));
                      return callback(request);
                  });
            }
            else
            {
                throw new InvalidOperationException($"Cannot start responding rpc type is invalid, rpc type: {_netqRabbitRpcConnection.RpcMessageType}");
            }

            _isStarted = true;
            _logger.LogInformation($"Started responding on bus: {_netqRabbitRpcConnection.Bus}, of type {_netqRabbitRpcConnection.RpcMessageType}");
        }

        public void RespondAsync<TRequest, TRespond>(Func<TRequest, Task<TRespond>> callback)
            where TRequest : class
            where TRespond : class
        {
            VerifyServerHasntStartedYet();
            if (_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Abstract)
            {
                _respondHandler = _netqRabbitRpcConnection.Bus.RespondAsync<TRequest, TRespond>(callback);
            }
            else if (_netqRabbitRpcConnection.RpcMessageType == RpcMessageType.Concrete)
            {
                _respondHandler = _netqRabbitRpcConnection.RespondAsync(func =>
                {
                    TRequest request = (TRequest)func(typeof(TRequest));
                    return callback(request);
                });
            }
            else
            {
                throw new InvalidOperationException($"Cannot start responding rpc type is invalid, rpc type: {_netqRabbitRpcConnection.RpcMessageType}");
            }

            _isStarted = true;
            _logger.LogInformation($"Started responding async on bus: {_netqRabbitRpcConnection.Bus}, of type {_netqRabbitRpcConnection.RpcMessageType}");
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _respondHandler.Dispose();
                _respondHandler = null;
                _isStarted = false;
                _logger.LogInformation($"Stoped responding on bus: {_netqRabbitRpcConnection.Bus}, of type {_netqRabbitRpcConnection.RpcMessageType}");
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
