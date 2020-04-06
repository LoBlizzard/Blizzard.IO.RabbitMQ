using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Blizzard.IO.RabbitMQ.Extensions
{
    public static class BusExtensions
    {
        public static IBus CreateBus(
            string host = "localhost", 
            string username = "guest", 
            string password = "guest", 
            ushort timeout = 10,
            string product = "",
            string platform = "" , 
            string virtualHost = "/", 
            int requestHeartbeat = 10, 
            int prefetchCount = 50, 
            bool publisherConfirms = false,
            bool persistentMessages = true)
        {
            var connectionString = $"host={host}";
            connectionString += username != "guest" ? $";username={username}" : string.Empty;
            connectionString += password != "guest" ? $";password={password}" : string.Empty;
            connectionString += timeout != 10 ? $";timeout={timeout}" : string.Empty;
            connectionString += product != "" ? $";product={product}" : string.Empty;
            connectionString += platform != "" ? $";platform={platform}" : string.Empty;
            connectionString += virtualHost != "/" ? $";virtualHost={virtualHost}" : string.Empty;
            connectionString += requestHeartbeat != 10 ? $";requestHeartbeat={requestHeartbeat}" : string.Empty;
            connectionString += prefetchCount != 50 ? $";prefetchCount={prefetchCount}" : string.Empty;
            connectionString += publisherConfirms != false ? $";publisherConfirms={publisherConfirms}" : string.Empty;
            connectionString += persistentMessages != true ? $";persistentMessages={persistentMessages}" : string.Empty;

            return RabbitHutch.CreateBus(connectionString);
        }

        public static RabbitExchange DeclareExchange(this IBus bus,
            string name,
            RabbitExchangeType type,
            bool passive = false,
            bool durable = true,
            bool autoDelete = false,
            bool @internal = false,
            string alternateExchange = null,
            bool delayed = false)
        {
            bus.Advanced.ExchangeDeclare(name, Utilities.ExchangeTypeToStringResolver[type], passive, durable,
                autoDelete, @internal, alternateExchange,
                delayed);

            return new RabbitExchange()
            {
                AlternateExchange = alternateExchange,
                AutoDelete = autoDelete,
                Delayed = delayed,
                Durable = durable,
                Internal = @internal,
                Name = name,
                Passive = passive,
                Type = type
            };
        }

        public static RabbitQueue DeclareQueue(this IBus bus, 
            string name,
            bool passive = false,
            bool durable = true,
            bool exclusive = false,
            bool autoDelete = false,
            int? perQueueMessageTtl = null,
            int? expires = null,
            int? maxPriority = null,
            string deadLetterExchange = null,
            string deadLetterRoutingKey = null,
            int? maxLength = null,
            int? maxLengthBytes = null)
        {
            bus.Advanced.QueueDeclare(name, passive, durable, exclusive, autoDelete, perQueueMessageTtl, expires,
                maxPriority, deadLetterExchange, deadLetterRoutingKey, maxLength, maxLengthBytes);

            return new RabbitQueue()
            {
                AutoDelete = autoDelete,
                DeadLetterExchange = deadLetterExchange,
                DeadLetterRoutingKey = deadLetterRoutingKey,
                Durable = durable,
                Exclusive = exclusive,
                Expires = expires,
                MaxLength = maxLength,
                MaxLengthBytes = maxLengthBytes,
                MaxPriority = maxPriority,
                Name = name,
                Passive = passive,
                PerQueueMessageTtl = perQueueMessageTtl
            };

        }

        public static RabbitBinding BindExchangeToQueue(this IBus bus, RabbitExchange sourceExchange, RabbitQueue destinationQueue, string routingKey ="/")
        {
            IExchange netqExchange = new Exchange(sourceExchange.Name);
            IQueue netqQueue = new Queue(destinationQueue.Name, destinationQueue.Exclusive);
            bus.Advanced.Bind(netqExchange, netqQueue, routingKey);
            return new RabbitBinding()
            {
                SourceName = sourceExchange.Name,
                DestName = destinationQueue.Name,
                RoutingKey = routingKey
            };
        }

        public static RabbitBinding BindExchangeToExchanges(this IBus bus, RabbitExchange sourceExchange, RabbitExchange destinationExchange, string routingKey = "/")
        {
            IExchange sourceNetqExchange = new Exchange(sourceExchange.Name);
            IExchange destinationNetqExchange = new Exchange(destinationExchange.Name);
            bus.Advanced.Bind(sourceNetqExchange, destinationNetqExchange, routingKey);
            return new RabbitBinding()
            {
                SourceName =  sourceExchange.Name,
                DestName = destinationExchange.Name,
                RoutingKey = routingKey
            };
        }
    }
}