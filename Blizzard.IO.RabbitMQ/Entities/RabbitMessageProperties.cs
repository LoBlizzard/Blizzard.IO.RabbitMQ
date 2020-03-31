using Blizzard.IO.Core;
using System;
using System.Collections.Generic;

namespace Blizzard.IO.RabbitMQ.Entities
{
    public class RabbitMessageProperties : BaseMetadata
    {
        public byte DeliveryMode { get; set; }
        public string Type { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public string MessageId { get; set; }
        public string CorellationId { get; set; }
        public string ReplyTo { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string AddId { get; set; }
        public string ClusterId { get; set; }
        public string Expiration { get; set; }
        public bool Persistent { get; set; }
        public byte Priority { get; set; }
    }
}
