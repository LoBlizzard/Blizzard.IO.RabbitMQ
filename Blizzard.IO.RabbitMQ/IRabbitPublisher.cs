using Blizzard.IO.RabbitMQ.Entities;

namespace Blizzard.IO.RabbitMQ
{
    public interface IRabbitPublisher<TData>
    {
        void Publish(TData data, string routingKey);
        void Publish(TData data, RabbitMessageProperties messageProperties, string routingKey);
    }
}
