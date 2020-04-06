using System.Collections.Generic;
using EasyNetQ;

namespace Blizzard.IO.RabbitMQ.Builders
{
    public abstract class BaseNetqRabbitBuilder
    {
        protected static Dictionary<string, IBus> NetqRabbitConnections { get; private set; }

        protected BaseNetqRabbitBuilder()
        {
            if (NetqRabbitConnections == null)
            {
                NetqRabbitConnections = new Dictionary<string, IBus>();
            }
        }

        protected IBus InitConnection(string host, string password, string username)
        {
            string busKey = $"{host};{username};{password}";
            if (NetqRabbitConnections.TryGetValue(busKey, out IBus bus))
            {
                return bus;
            }

            NetqRabbitConnections.Add(busKey, )
        }

        private IBus CreateBus(string host, string password, string username)
        {
            return RabbitHutch.CreateBus()
        }
    }
}
