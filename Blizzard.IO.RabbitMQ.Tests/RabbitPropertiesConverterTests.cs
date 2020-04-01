using NUnit.Framework;
using EasyNetQ;
using System.Collections.Generic;
using Blizzard.IO.RabbitMQ.Entities;

namespace Blizzard.IO.RabbitMQ.Tests
{
    [TestFixture]
    public class RabbitPropertiesConverterTests
    {
        private RabbitPropertiesConverter _rabbitPropertiesConverter;

        [SetUp]
        public void SetUp()
        {
            _rabbitPropertiesConverter = new RabbitPropertiesConverter();
        }

        [Test]
        public void Convert_OnConvertingCertainEasyNetQProperties_ShouldCreateRabbitMessagePropertiesWithSameValues()
        {
            // Arrange
            var netqRabbitProperties = new MessageProperties
            {
                DeliveryMode = 10,
                Type = "0",
                Headers = new Dictionary<string, object>(),
                ContentType = "1",
                ContentEncoding = "2",
                MessageId = "3",
                CorrelationId = "4",
                ReplyTo = "5",
                Timestamp = 6,
                UserId = "7",
                AppId = "8",
                ClusterId = "9",
                Expiration = "10",
                Priority = 11
            };

            // Act
            RabbitMessageProperties properties = _rabbitPropertiesConverter.Convert(netqRabbitProperties);

            // Assert
            Assert.That(properties.DeliveryMode, Is.EqualTo(10));
            Assert.That(properties.Type, Is.EqualTo("0"));
            Assert.That(properties.Headers, Is.EqualTo(netqRabbitProperties.Headers));
            Assert.That(properties.ContentType, Is.EqualTo("1"));
            Assert.That(properties.ContentEncoding, Is.EqualTo("2"));
            Assert.That(properties.MessageId, Is.EqualTo("3"));
            Assert.That(properties.CorrelationId, Is.EqualTo("4"));
            Assert.That(properties.ReplyTo, Is.EqualTo("5"));
            Assert.That(properties.Timestamp, Is.EqualTo(6));
            Assert.That(properties.UserId, Is.EqualTo("7"));
            Assert.That(properties.AppId, Is.EqualTo("8"));
            Assert.That(properties.ClusterId, Is.EqualTo("9"));
            Assert.That(properties.Expiration, Is.EqualTo("10"));
            Assert.That(properties.Priority, Is.EqualTo(11));
        }

        [Test]
        public void Convert_OnConvertingCertainRabbitMessageProperties_ShouldCreateEasyNetQPropertiesWithSameValues()
        {
            // Arrange
            var netqRabbitProperties = new RabbitMessageProperties
            {
                DeliveryMode = 10,
                Type = "0",
                Headers = new Dictionary<string, object>(),
                ContentType = "1",
                ContentEncoding = "2",
                MessageId = "3",
                CorrelationId = "4",
                ReplyTo = "5",
                Timestamp = 6,
                UserId = "7",
                AppId = "8",
                ClusterId = "9",
                Expiration = "10",
                Priority = 11
            };

            // Act
            MessageProperties properties = _rabbitPropertiesConverter.Convert(netqRabbitProperties);

            // Assert
            Assert.That(properties.DeliveryMode, Is.EqualTo(10));
            Assert.That(properties.Type, Is.EqualTo("0"));
            Assert.That(properties.Headers, Is.EqualTo(netqRabbitProperties.Headers));
            Assert.That(properties.ContentType, Is.EqualTo("1"));
            Assert.That(properties.ContentEncoding, Is.EqualTo("2"));
            Assert.That(properties.MessageId, Is.EqualTo("3"));
            Assert.That(properties.CorrelationId, Is.EqualTo("4"));
            Assert.That(properties.ReplyTo, Is.EqualTo("5"));
            Assert.That(properties.Timestamp, Is.EqualTo(6));
            Assert.That(properties.UserId, Is.EqualTo("7"));
            Assert.That(properties.AppId, Is.EqualTo("8"));
            Assert.That(properties.ClusterId, Is.EqualTo("9"));
            Assert.That(properties.Expiration, Is.EqualTo("10"));
            Assert.That(properties.Priority, Is.EqualTo(11));
        }
    }
}
