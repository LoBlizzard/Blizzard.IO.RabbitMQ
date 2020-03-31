namespace Blizzard.IO.RabbitMQ.Entities
{
    public enum RabbitExchangeType
    {
        Fanout,
        Direct,
        Topic,
        Headers,
        XConsistentHash
    }
}
