﻿using Blizzard.IO.RabbitMQ.Entities.Rpc;
using EasyNetQ;
using EasyNetQ.Producer;
using System;
using ISerializer = Blizzard.IO.Core.Rpc.ISerializer;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Rpc
{
    public class NetqRabbitRpcConnection : INetqRabbitRpcConnection
    {
        public IBus Bus { get; }
        public RpcMessageType RpcMessageType { get; }
        private readonly ILogger<NetqRabbitRpcConnection> _logger;

        public NetqRabbitRpcConnection(RpcConfiguration configuration, string hostname, string username, string password, ILoggerFactory loggerFactory,
            ISerializer serializer, int heartBeat = 10, int preFetch = 50, ushort timeout = 10,bool publisherConfirms = false, bool persistent = true, 
            string product = null, string platform = null, string virtualHost = null, RpcMessageType rpcMessageType = RpcMessageType.Concrete)
        {
            _logger = loggerFactory.CreateLogger<NetqRabbitRpcConnection>();
            RpcMessageType = rpcMessageType;

            string connectionString = GetConnectionString(hostname, username, password, heartBeat, preFetch, timeout, publisherConfirms, persistent,
                product, platform, virtualHost);

            Bus = RabbitHutch.CreateBus(connectionString, registerer =>
            {
                switch (rpcMessageType)
                {
                    case RpcMessageType.Abstract:
                        registerer.Register<IMessageSerializationStrategy, AbstractMessageSerializationStrategy>();
                        break;
                    default:
                        registerer.Register<IMessageSerializationStrategy, ConcreteMessageSerializationStrategy>();
                        break;
                }

                registerer.Register(configuration);
                registerer.Register<IRpc, RpcRabbitWrapper>();
                registerer.Register(serializer);
            });

            _logger.LogInformation($"Created new RpcRabbit connection to host: {hostname}, of type: {rpcMessageType}");
        }

        private string GetConnectionString(string hostname, string username, string password, int heartBeat, int preFetch, ushort timeout,
            bool publisherConfirms, bool persistent, string product,string platform, string virtualHost)
        {
            string connectionString = $"host={hostname};username={username};password={password};requestedHeartbeat={heartBeat};prefetchcount={preFetch};" +
                $"persistentMessages={persistent};publisherConfirms={publisherConfirms};timeout={timeout}";
            if (product != null)
            {
                connectionString += $";product={product}";
            }
            if (platform != null)
            {
                connectionString += $";platform={platform}";
            }
            if (virtualHost != null)
            {
                connectionString += $";virtualHost={virtualHost}";
            }

            return connectionString;
        }

        public void Dispose()
        {
            Bus.Dispose();
        }

        public Func<Type, object> Request<TRequest>(TRequest request)
            where TRequest:class
        {
            return Bus.Request<TRequest,Func<Type,object>>(request);
        }

        public Task<Func<Type, object>> RequestAsync<TRequest>(TRequest request) 
            where TRequest : class
        {
            return Bus.RequestAsync<TRequest, Func<Type, object>>(request);
        }

        public IDisposable Respond<TRespond>(Func<Func<Type, object>, TRespond> callback) 
            where TRespond : class
        {
            return Bus.Respond(callback);
        }

        public IDisposable RespondAsync<TRespond>(Func<Func<Type, object>, Task<TRespond>> callback) 
            where TRespond : class
        {
            return Bus.RespondAsync(callback);
        }
    }
}
