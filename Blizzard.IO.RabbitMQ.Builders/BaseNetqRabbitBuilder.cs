using System.Collections.Generic;
using EasyNetQ;
using Blizzard.IO.RabbitMQ.Extensions;

namespace Blizzard.IO.RabbitMQ.Builders
{
    public abstract class BaseNetqRabbitBuilder
    {
        private static Dictionary<ConnectionKey, IBus> netqRabbitConnections = new Dictionary<ConnectionKey, IBus>();

        protected IBus InitConnection(
            string host,
            string password,
            string username,
            ushort timeout = 10,
            string product = "",
            string platform = "",
            string virtualHost = "/",
            int requestHeartbeat = 10,
            int prefetchCount = 50,
            bool publisherConfirms = false,
            bool persistentMessages = true)
        {
            ConnectionKey busKey = new ConnectionKey(host, username, password);
            if (netqRabbitConnections.ContainsKey(busKey))
            {
                return netqRabbitConnections[busKey];
            }

            IBus bus = BusExtensions.CreateBus(host, username, password, timeout, product, platform, virtualHost, 
                requestHeartbeat, prefetchCount, publisherConfirms, persistentMessages);
            netqRabbitConnections.Add(busKey, bus);

            return bus;
        }
    }
}
