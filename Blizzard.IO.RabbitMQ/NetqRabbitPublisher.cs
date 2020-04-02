using System;
using Blizzard.IO.RabbitMQ.Entities;
using Blizzard.IO.Core;
using EasyNetQ;
using EasyNetQ.Topology;

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

        public NetqRabbitPublisher(IBus netqBus, ISerializer<TData> serializer, RabbitExchange destinationExchange, bool isAbstract = false, string routingKey = "/")
        {
            _netqBus = netqBus;
            _serializer = serializer;
            _destinationExchange = _netqBus.Advanced.ExchangeDeclare(destinationExchange.Name.ToLower(),
                destinationExchange.Type.ToString(), destinationExchange.Passive, destinationExchange.Durable,
                destinationExchange.AutoDelete, destinationExchange.Internal, destinationExchange.AlternateExchange,
                destinationExchange.Delayed);

            _isAbstract = isAbstract;
            _routingKey = routingKey;
            _rabbitMessagePropertiesToMessagePropertiesConverter = new RabbitPropertiesConverter();
        }

        public void Publish(TData data)
        {
            byte[] body = _serializer.Serialize(data);
            MessageProperties netqMessageProperties = new MessageProperties();
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());
            _netqBus.Advanced.Publish(_destinationExchange, _routingKey, false, netqMessageProperties, body);
        }

        public void Publish(TData data, string routingKey)
        {
            byte[] body = _serializer.Serialize(data);
            var netqMessageProperties = new MessageProperties();
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());
            _netqBus.Advanced.Publish(_destinationExchange, routingKey, false, netqMessageProperties, body);
        }

        public void Publish(TData data, RabbitMessageProperties rabbitMessageProperties)
        {
            byte[] body = _serializer.Serialize(data);
            MessageProperties netqMessageProperties = _rabbitMessagePropertiesToMessagePropertiesConverter.Convert(rabbitMessageProperties);
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());
            _netqBus.Advanced.Publish(_destinationExchange, _routingKey, false, netqMessageProperties, body);
        }

        public void Publish(TData data, RabbitMessageProperties rabbitMessageProperties, string routingKey)
        {
            byte[] body = _serializer.Serialize(data);
            MessageProperties netqMessageProperties = _rabbitMessagePropertiesToMessagePropertiesConverter.Convert(rabbitMessageProperties);
            EnrichWithTypeIfAbstract(netqMessageProperties, data.GetType());

            _netqBus.Advanced.Publish(_destinationExchange, routingKey, false, netqMessageProperties, body);
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
