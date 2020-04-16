using Blizzard.IO.Core;
using System;
using System.Collections.Generic;

namespace Blizzard.IO.RabbitMQ.Entities
{
    public class RabbitMessageProperties : BaseMetadata
    {
        public byte? DeliveryMode { get; set; }
        public bool DeliveryModePresent => DeliveryMode != null;

        public string Type { get; set; }
        public bool TypePresent => Type != null;

        public IDictionary<string, object> Headers { get; set; }
        public bool HeadersPresent => Headers != null;

        public string ContentType { get; set; }
        public bool ContentTypePresent => ContentType != null;

        public string ContentEncoding { get; set; }
        public bool ContentEncodingPresent => ContentEncoding != null;


        public string MessageId { get; set; }
        public bool MessageIdPresent => MessageId != null;

        public string CorrelationId { get; set; }
        public bool CorrelationIdPresent => CorrelationId != null;

        public string ReplyTo { get; set; }
        public bool ReplyToPresent => ReplyTo != null;

        public long? Timestamp { get; set; }
        public bool TimestampPresent => Timestamp != null;

        public string UserId { get; set; }
        public bool UserIdPresent => UserId != null;

        public string AppId { get; set; }
        public bool AppIdPresent => AppId != null;

        public string ClusterId { get; set; }
        public bool ClusterIdPresent => ClusterId != null;

        public string Expiration { get; set; }
        public bool ExpirationPresent => Expiration != null;

        public byte? Priority { get; set; }
        public bool PriorityPresent => Priority != null;
    }
}
