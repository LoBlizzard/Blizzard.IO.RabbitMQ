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
                AppIdPresent = rabbitMessageProperties.AppIdPresent,
                ClusterId = rabbitMessageProperties.ClusterId,
                ClusterIdPresent = rabbitMessageProperties.ClusterIdPresent,
                ContentEncoding = rabbitMessageProperties.ContentEncoding,
                ContentEncodingPresent = rabbitMessageProperties.ContentEncodingPresent,
                ContentType = rabbitMessageProperties.ContentType,
                ContentTypePresent = rabbitMessageProperties.ContentTypePresent,
                CorrelationId = rabbitMessageProperties.CorrelationId,
                CorrelationIdPresent = rabbitMessageProperties.CorrelationIdPresent,
                DeliveryMode = rabbitMessageProperties.DeliveryModePresent ? (byte)rabbitMessageProperties.DeliveryMode: (byte)0,
                DeliveryModePresent = rabbitMessageProperties.DeliveryModePresent,
                Type = rabbitMessageProperties.Type,
                TypePresent = rabbitMessageProperties.TypePresent,
                MessageId = rabbitMessageProperties.MessageId,
                MessageIdPresent = rabbitMessageProperties.MessageIdPresent,
                Headers = rabbitMessageProperties.Headers,
                HeadersPresent = rabbitMessageProperties.HeadersPresent,
                Timestamp = rabbitMessageProperties.TimestampPresent ? (long)rabbitMessageProperties.Timestamp : 0,
                TimestampPresent = rabbitMessageProperties.TimestampPresent,
                Priority = rabbitMessageProperties.PriorityPresent ? (byte)rabbitMessageProperties.Priority : (byte)0,
                PriorityPresent = rabbitMessageProperties.PriorityPresent,
                UserId = rabbitMessageProperties.UserId,
                UserIdPresent = rabbitMessageProperties.UserIdPresent,
                Expiration = rabbitMessageProperties.Expiration,
                ExpirationPresent = rabbitMessageProperties.ExpirationPresent,
                ReplyTo = rabbitMessageProperties.ReplyTo,
                ReplyToPresent = rabbitMessageProperties.ReplyToPresent
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
