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
            return new MessageProperties()
            {
                AppId = rabbitMessageProperties.AppId,
                AppIdPresent = rabbitMessageProperties.AppId!= null,
                ClusterId = rabbitMessageProperties.ClusterId,
                ClusterIdPresent = rabbitMessageProperties.ClusterId != null,
                ContentEncoding = rabbitMessageProperties.ContentEncoding,
                ContentEncodingPresent = rabbitMessageProperties.ContentEncoding != null,
                ContentType = rabbitMessageProperties.ContentType,
                ContentTypePresent = rabbitMessageProperties.ContentType != null,
                CorrelationId = rabbitMessageProperties.CorrelationId,
                CorrelationIdPresent = rabbitMessageProperties.CorrelationId != null,
                DeliveryMode = rabbitMessageProperties.DeliveryMode ?? 0 ,
                DeliveryModePresent = rabbitMessageProperties.DeliveryMode != null,
                Type = rabbitMessageProperties.Type,
                TypePresent = rabbitMessageProperties.Type != null,
                MessageId = rabbitMessageProperties.MessageId,
                MessageIdPresent = rabbitMessageProperties.MessageId != null,
                Headers = rabbitMessageProperties.Headers,
                HeadersPresent = rabbitMessageProperties.Headers != null,
                Timestamp = rabbitMessageProperties.Timestamp ?? 0,
                TimestampPresent = rabbitMessageProperties.Timestamp != null,
                Priority = rabbitMessageProperties.Priority ?? 0,
                PriorityPresent = rabbitMessageProperties.Priority != null,
                UserId = rabbitMessageProperties.UserId,
                UserIdPresent = rabbitMessageProperties.UserId != null,
                Expiration = rabbitMessageProperties.Expiration,
                ExpirationPresent = rabbitMessageProperties.Expiration != null,
                ReplyTo = rabbitMessageProperties.ReplyTo,
                ReplyToPresent = rabbitMessageProperties.ReplyTo != null
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
