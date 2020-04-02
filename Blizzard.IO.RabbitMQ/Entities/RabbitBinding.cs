namespace Blizzard.IO.RabbitMQ.Entities
{
    /// <summary>
    /// Represents a binding topology between source and dest, both source and dest may be exchanges or queues.
    /// </summary>
    public class RabbitBinding
    {
        public string SourceName { get; set; }
        public string DestName { get; set; }
        public string RoutingKey { get; set; }
    }
}
