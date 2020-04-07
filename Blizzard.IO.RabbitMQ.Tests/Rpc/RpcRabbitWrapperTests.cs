using Blizzard.IO.RabbitMQ.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using NUnit.Framework;
using EasyNetQ;
using Moq;
using EasyNetQ.Producer;
using System;
using EasyNetQ.Topology;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Tests.Rpc
{
    class CancelationMock : IDisposable
    {
        public void Dispose()
        {}
    }

    class ConventionStub : IConventions
    {
        public ExchangeNameConvention ExchangeNamingConvention { get; set; }
        public TopicNameConvention TopicNamingConvention { get; set; }
        public QueueNameConvention QueueNamingConvention { get; set; }
        public RpcRoutingKeyNamingConvention RpcRoutingKeyNamingConvention { get; set; }
        public ErrorQueueNameConvention ErrorQueueNamingConvention { get; set; }
        public ErrorExchangeNameConvention ErrorExchangeNamingConvention { get; set; }
        public RpcExchangeNameConvention RpcRequestExchangeNamingConvention { get; set; }
        public RpcExchangeNameConvention RpcResponseExchangeNamingConvention { get; set; }
        public RpcReturnQueueNamingConvention RpcReturnQueueNamingConvention { get; set; }
        public ConsumerTagConvention ConsumerTagConvention { get; set; }
    }

    [TestFixture]
    public class RpcRabbitWrapperTests
    {
        private RpcRabbitWrapper _rpcRabbitWrapper;
        private RpcConfiguration _rpcConfiguration;

        private Mock<IAdvancedBus> _advancedBusMock;
        private Mock<IEventBus> _eventBusMock;
        private ConventionStub _conventionsStub;
        private Mock<IExchangeDeclareStrategy> _exchangeDeclareStrategyMock;
        private Mock<IMessageDeliveryModeStrategy> _messageDelveryStrategyMock;
        private Mock<ITimeoutStrategy> _timeoutStrategyMock;
        private Mock<ITypeNameSerializer> _typenameSerializerMock;

        private bool _calledRequestExchangeNameProvider = false;
        private bool _calledRoutingKeyProvider = false;
        private bool _calledResponseExchangeNameProvider = false;
        private bool _calledReturnQueueNameProvider = false;

        [SetUp]
        public void Setup()
        {
            _rpcConfiguration = new RpcConfiguration
            {
                RequestExchangeNameProvider = type => 
                {
                    _calledRequestExchangeNameProvider = true;
                    return "test1";
                },
                RoutingKeyProvider = type =>
                {
                    _calledRoutingKeyProvider = true;
                    return "test2";
                },
                ResponseExchangeNameProvider= type =>
                {
                    _calledResponseExchangeNameProvider = true;
                    return "test3";
                },
                ReturnQueueNameProvider = () =>
                {
                    _calledReturnQueueNameProvider= true;
                    return "test4";
                }
            };

            _advancedBusMock = new Mock<IAdvancedBus>();
            _eventBusMock = new Mock<IEventBus>();
            _conventionsStub = new ConventionStub();
            _exchangeDeclareStrategyMock = new Mock<IExchangeDeclareStrategy>();
            _messageDelveryStrategyMock = new Mock<IMessageDeliveryModeStrategy>();
            _timeoutStrategyMock = new Mock<ITimeoutStrategy>();
            _typenameSerializerMock = new Mock<ITypeNameSerializer>();

            ConnectionConfiguration config = new ConnectionConfiguration();

            _rpcRabbitWrapper = new RpcRabbitWrapper(_rpcConfiguration, config, _advancedBusMock.Object, _eventBusMock.Object, _conventionsStub, 
                _exchangeDeclareStrategyMock.Object,_messageDelveryStrategyMock.Object, _timeoutStrategyMock.Object, _typenameSerializerMock.Object);
        }

        [Test]
        public void Ctor_OnRpcWrapperUsage_ShouldUseRpcConfigurationToGenerateExchangesAndQueuesInsteadOfTheDefaultNetqWay()
        {
            //Arrange
            _advancedBusMock.Setup(bus => bus.Consume<object>(It.IsAny<IQueue>(), It.IsAny<Func<IMessage<object>,MessageReceivedInfo,Task>>()))
                .Returns(new CancelationMock());
            _advancedBusMock.Setup(bus => bus.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), 
                It.IsAny<bool>(), It.IsAny<bool>(),It.IsAny<int?>(),It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(new Queue("c",false));
            _advancedBusMock.Setup(bus => bus.ExchangeDeclare(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()
                , It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(new Exchange("e"));
            _advancedBusMock.Setup(bus => bus.Bind(It.IsAny<IExchange>(), It.IsAny<IQueue>(), It.IsAny<string>()));
            _timeoutStrategyMock.Setup(str => str.GetTimeoutSeconds(It.IsAny<Type>())).Returns(400);
            _messageDelveryStrategyMock.Setup(mdm => mdm.GetDeliveryMode(It.IsAny<Type>())).Returns(10);

            //Act
            var task = _rpcRabbitWrapper.Request<object, object>(new object(), x=> { });

            //Assert
            Assert.True(_calledRequestExchangeNameProvider);
            Assert.True(_calledRoutingKeyProvider);
            Assert.True(_calledResponseExchangeNameProvider);
            Assert.True(_calledReturnQueueNameProvider);
        }
    }
}
