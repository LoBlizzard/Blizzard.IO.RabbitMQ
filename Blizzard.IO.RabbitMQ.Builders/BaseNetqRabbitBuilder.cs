using System.Collections.Generic;
using EasyNetQ;
using Blizzard.IO.RabbitMQ.Extensions;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ.Builders
{
    public abstract class BaseNetqRabbitBuilder
    {
        private static Dictionary<ConnectionKey, IBus> netqRabbitConnections = new Dictionary<ConnectionKey, IBus>();

        protected string Hostname = "localhost";
        protected string Password = "guest";
        protected string Username = "guest";
        protected ushort Timeout = 10;
        protected string Product = "";
        protected string Platform = "";
        protected string VirtualHost = "/";
        protected int RequestHeartbeat = 10;
        protected int PrefetchCount = 50;
        protected bool PublisherConfirms = false;
        protected bool PersistentMessages = true;
        protected readonly ILoggerFactory LoggerFactory;

        protected BaseNetqRabbitBuilder(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        protected IBus InitConnection()
        {
            var busKey = new ConnectionKey(Hostname,Username, Password);
            if (netqRabbitConnections.ContainsKey(busKey))
            {
                return netqRabbitConnections[busKey];
            }

            IBus bus = BusExtensions.CreateBus(Hostname, Username, Password, Timeout, Product, Platform, VirtualHost, 
                RequestHeartbeat, PrefetchCount, PublisherConfirms, PersistentMessages);
            netqRabbitConnections.Add(busKey, bus);

            return bus;
        }

        public static void CloseConnections()
        {
            foreach (var connection in netqRabbitConnections.Values)
            {
                connection.Dispose();
            }
        }
    }
}
