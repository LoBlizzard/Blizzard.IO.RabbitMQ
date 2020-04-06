using Blizzard.IO.RabbitMQ.Entities.Rpc;
using System;

namespace Blizzard.IO.RabbitMQ.Builders.Rpc
{
    public struct RpcConnectionKey
    {
        public string Hostname { get; }
        public string Username { get; }
        public string Password { get; }
        public RpcMessageType RpcMessageType { get; }
        public Type SerializerType { get; }

        public RpcConnectionKey(string hostname, string username, string password, RpcMessageType rpcMessageType, Type serializerType)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
            RpcMessageType = rpcMessageType;
            SerializerType = serializerType;
        }
    }
}
