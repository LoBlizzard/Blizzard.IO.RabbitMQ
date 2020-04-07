using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using Microsoft.Extensions.Logging;

namespace Blizzard.IO.RabbitMQ.Builders
{
    public class NetqRabbitPublisherBuilder<TData> : BaseNetqRabbitBuilder
    {
        private RabbitExchange _destinationExchange = null;
        private bool _isAbstract = false;
        private ISerializer<TData> _serializer = null;
        private string _routingKey = "/";
        private IConverter<RabbitMessageProperties, MessageProperties> _converter = null;
        private ILoggerFactory _loggerFactory = null;

        public NetqRabbitPublisherBuilder<TData> AddSerializer(ISerializer<TData> serializer)
        {
            _serializer = serializer;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddAbstractSupport()
        {
            _isAbstract = true;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddDestinationExchange(RabbitExchange destinationExchange)
        {
            _destinationExchange = destinationExchange;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddRoutingKey(string routingKey)
        {
            _routingKey = routingKey;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        public NetqRabbitPublisherBuilder<TData> AddConverter(
            IConverter<RabbitMessageProperties, MessageProperties> converter)
        {
            _converter = converter;
            return this;
        }

        public NetqRabbitPublisher<TData> BuildPublisher()
        {
            IBus bus = InitConnection();
            return new NetqRabbitPublisher<TData>(bus, _serializer, _destinationExchange, _loggerFactory, _converter,
                _isAbstract, _routingKey);
        }

    }
}