using System;
using Blizzard.IO.RabbitMQ.Entities;
using Blizzard.IO.Core;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ
{
    public class NetqRabbitPublisher<TData> : IRabbitPublisher<TData>, IPublisherWithMetadata<TData, RabbitMessageProperties>, IPublisher<TData>
    {
        private readonly IBus _netqBus;
        private readonly ISerializer<TData> _serializer;
        private readonly IExchange _destinationExchange;
        private readonly IConverter<RabbitMessageProperties, MessageProperties> _rabbitMessagePropertiesToMessagePropertiesConverter;
        private readonly string _routingKey;
        private readonly bool _isAbstract;
        private readonly ILogger _logger;

        public NetqRabbitPublisher(IBus netqBus, ISerializer<TData> serializer,
            RabbitExchange destinationExchange, ILoggerFactory loggerFactory, IConverter<RabbitMessageProperties, MessageProperties> rabbitMessagePropertiesToMessagePropertiesConverter = null,
             bool isAbstract = false, string routingKey = "/")
        {
            _netqBus = netqBus;
            _serializer = serializer;
            _destinationExchange = new Exchange(destinationExchange.Name);

            _isAbstract = isAbstract;
            _routingKey = routingKey;
            _rabbitMessagePropertiesToMessagePropertiesConverter = rabbitMessagePropertiesToMessagePropertiesConverter ?? new RabbitPropertiesConverter();

            _logger = loggerFactory.CreateLogger(nameof(NetqRabbitPublisher<TData>));
        }

        public void Publish(TData data)
        {
            byte[] body = _serializer.Serialize(data);
            MessageProperties netqMessageProperties = new MessageProperties();
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());
            _netqBus.Advanced.Publish(_destinationExchange, _routingKey, false, netqMessageProperties, body);
            _logger.LogInformation($"Published Message. Exchange: {_destinationExchange.Name} RoutingKey: {_routingKey} .");
        }

        public void Publish(TData data, string routingKey)
        {
            byte[] body = _serializer.Serialize(data);
            var netqMessageProperties = new MessageProperties();
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());
            _netqBus.Advanced.Publish(_destinationExchange, routingKey, false, netqMessageProperties, body);
            _logger.LogInformation($"Published Message. Exchange: {_destinationExchange.Name} RoutingKey: {routingKey} .");
        }

        public void Publish(TData data, RabbitMessageProperties rabbitMessageProperties)
        {
            byte[] body = _serializer.Serialize(data);
            MessageProperties netqMessageProperties = _rabbitMessagePropertiesToMessagePropertiesConverter.Convert(rabbitMessageProperties);
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());
            _netqBus.Advanced.Publish(_destinationExchange, _routingKey, false, netqMessageProperties, body);
            _logger.LogInformation($"Published Message. Exchange: {_destinationExchange.Name} RoutingKey: {_routingKey} .");
        }

        public void Publish(TData data, RabbitMessageProperties rabbitMessageProperties, string routingKey)
        {
            byte[] body = _serializer.Serialize(data);
            MessageProperties netqMessageProperties = _rabbitMessagePropertiesToMessagePropertiesConverter.Convert(rabbitMessageProperties);
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());
            _netqBus.Advanced.Publish(_destinationExchange, routingKey, false, netqMessageProperties, body);
            _logger.LogInformation($"Published Message. Exchange: {_destinationExchange.Name} RoutingKey: {routingKey} .");
        }

        private void EnrichWithTypeIfAbstract(MessageProperties messageProperties, Type dataType)
        {
            if (_isAbstract)
            {
                messageProperties.Type = dataType.ToString();
            }
        }
    }
}
