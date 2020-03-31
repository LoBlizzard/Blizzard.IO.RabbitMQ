namespace Blizzard.IO.RabbitMQ.Entities
{
    public class RabbitExchange
    {
        public string Name { get; set; }
        public RabbitExchangeType Type { get; set; }
        public bool Passive { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public bool Internal { get; set; }
        public string AlternateExchange { get; set; }
        public bool Delayed { get; set; }
    }
}
