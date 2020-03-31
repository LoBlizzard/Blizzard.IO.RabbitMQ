using Blizzard.IO.RabbitMQ.Entities;

namespace Blizzard.IO.RabbitMQ
{
    public interface IRabbitConfigurator
    {
        void Configure(RabbitConfiguration configuration);
    }
}
