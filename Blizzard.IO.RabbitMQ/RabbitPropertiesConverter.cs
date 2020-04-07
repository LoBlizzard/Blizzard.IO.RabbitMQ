using Blizzard.IO.Core;
using Blizzard.IO.RabbitMQ.Entities;
using EasyNetQ;

namespace Blizzard.IO.RabbitMQ
{
    public class RabbitPropertiesConverter :
        IConverter<RabbitMessageProperties, MessageProperties>,
        IConverter<MessageProperties, RabbitMessageProperties>
    {
        public MessageProperties Convert(RabbitMessageProperties rabbitMessageProperties)
        {
            var messageProperties = new MessageProperties();

            if (rabbitMessageProperties.DeliveryMode != null)
            {
                messageProperties.DeliveryMode = (byte) rabbitMessageProperties.DeliveryMode;
            }

            if (rabbitMessageProperties.Type != null)
            {
                messageProperties.Type = rabbitMessageProperties.Type;
            }

            if (rabbitMessageProperties.Headers != null)
            {
                messageProperties.Headers = rabbitMessageProperties.Headers;
            }
            else
            {
                // By default the headersPresent is set to true
                messageProperties.HeadersPresent = false;
            }

            if (rabbitMessageProperties.ContentType != null)
            {
                messageProperties.ContentType = rabbitMessageProperties.ContentType;
            }

            if (rabbitMessageProperties.ContentEncoding != null)
            {
                messageProperties.ContentEncoding = rabbitMessageProperties.ContentEncoding;
            }

            if (rabbitMessageProperties.MessageId != null)
            {
                messageProperties.MessageId = rabbitMessageProperties.MessageId;
            }

            if (rabbitMessageProperties.CorrelationId != null)
            {
                messageProperties.CorrelationId = rabbitMessageProperties.CorrelationId;
            }

            if (rabbitMessageProperties.ReplyTo != null)
            {
                messageProperties.ReplyTo = rabbitMessageProperties.ReplyTo;
            }

            if (rabbitMessageProperties.Timestamp != null)
            {
                messageProperties.Timestamp = (long) rabbitMessageProperties.Timestamp;
            }

            if (rabbitMessageProperties.UserId != null)
            {
                messageProperties.UserId = rabbitMessageProperties.UserId;
            }

            if (rabbitMessageProperties.AppId != null)
            {
                messageProperties.AppId = rabbitMessageProperties.AppId;
            }

            if (rabbitMessageProperties.ClusterId != null)
            {
                messageProperties.ClusterId = rabbitMessageProperties.ClusterId;
            }

            if (rabbitMessageProperties.Expiration != null)
            {
                messageProperties.Expiration = rabbitMessageProperties.Expiration;
            }

            if (rabbitMessageProperties.Priority != null)
            {
                messageProperties.Priority = (byte) rabbitMessageProperties.Priority;
            }

            return messageProperties;
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
