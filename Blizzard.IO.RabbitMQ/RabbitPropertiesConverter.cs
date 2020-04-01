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
        public MessageProperties Convert(RabbitMessageProperties rabbitMessageProperties)
        {
            return new MessageProperties
            {
                DeliveryMode = rabbitMessageProperties.DeliveryMode,
                Type = rabbitMessageProperties.Type,
                Headers = rabbitMessageProperties.Headers,
                ContentType = rabbitMessageProperties.ContentType,
                ContentEncoding = rabbitMessageProperties.ContentEncoding,
                MessageId = rabbitMessageProperties.MessageId,
                CorrelationId = rabbitMessageProperties.CorrelationId,
                ReplyTo = rabbitMessageProperties.ReplyTo,
                Timestamp = rabbitMessageProperties.Timestamp,
                UserId = rabbitMessageProperties.UserId,
                AppId = rabbitMessageProperties.AppId,
                ClusterId = rabbitMessageProperties.ClusterId,
                Expiration = rabbitMessageProperties.Expiration,
                Priority = rabbitMessageProperties.Priority
            };
        }

        public RabbitMessageProperties Convert(MessageProperties messageProperties)
        {
            return new RabbitMessageProperties
            {
                DeliveryMode = messageProperties.DeliveryMode,
                Type = messageProperties.Type,
                Headers = messageProperties.Headers,
                ContentType = messageProperties.ContentType,
                ContentEncoding = messageProperties.ContentEncoding,
                MessageId = messageProperties.MessageId,
                CorrelationId = messageProperties.CorrelationId,
                ReplyTo = messageProperties.ReplyTo,
                Timestamp = messageProperties.Timestamp,
                UserId = messageProperties.UserId,
                AppId = messageProperties.AppId,
                ClusterId = messageProperties.ClusterId,
                Expiration = messageProperties.Expiration,
                Priority = messageProperties.Priority
            };
        }
    }
}
