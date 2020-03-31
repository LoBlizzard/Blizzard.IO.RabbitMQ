namespace Blizzard.IO.RabbitMQ.Entities
{
    public class RabbitQueue
    {
        public string Name { get; set; }
        public bool Passive { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public int? PerQueueMessageTtl { get; set; }
        public int? Expires { get; set; }
        public int? MaxPriority { get; set; }
        public string DeadLetterExchange { get; set; }
        public string DeadLetterRoutingKey { get; set; }
        public int? MaxLength { get; set; }
        public int? MaxLengthBytes { get; set; }
    }
}
