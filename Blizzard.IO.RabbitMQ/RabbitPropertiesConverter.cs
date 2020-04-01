using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;
using System;
using System.Collections.Generic;

namespace Blizzard.IO.RabbitMQ
{
    public class RabbitPropertiesConverter :
        IConverter<RabbitMessageProperties, MessageProperties>,
        IConverter<MessageProperties, RabbitMessageProperties>
    {
        public MessageProperties Convert(RabbitMessageProperties obj)
        {
            return new MessageProperties
            {
                DeliveryMode = obj.DeliveryMode,
                Type = obj.Type,
                Headers = obj.Headers as Dictionary<string, object>,
                ContentType = obj.ContentType,
                ContentEncoding = obj.ContentEncoding,
                MessageId = obj.MessageId,
                CorrelationId = obj.CorrelationId,
                ReplyTo = obj.ReplyTo,
                Timestamp = obj.Timestamp,
                UserId = obj.UserId,
                AppId = obj.AppId,
                ClusterId = obj.ClusterId,
                Expiration = obj.Expiration,
                Priority = obj.Priority
            };
        }

        public RabbitMessageProperties Convert(MessageProperties obj)
        {
            return new RabbitMessageProperties
            {
                DeliveryMode = obj.DeliveryMode,
                Type = obj.Type,
                Headers = obj.Headers as Dictionary<string, object>,
                ContentType = obj.ContentType,
                ContentEncoding = obj.ContentEncoding,
                MessageId = obj.MessageId,
                CorrelationId = obj.CorrelationId,
                ReplyTo = obj.ReplyTo,
                Timestamp = obj.Timestamp,
                UserId = obj.UserId,
                AppId = obj.AppId,
                ClusterId = obj.ClusterId,
                Expiration = obj.Expiration,
                Priority = obj.Priority
            };
        }
    }
}
