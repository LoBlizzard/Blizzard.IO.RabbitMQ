using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Blizzard.IO.RabbitMQ.Rpc;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Blizzard.IO.RabbitMQ.Tests.Stubs;

namespace Blizzard.IO.RabbitMQ.Tests.Rpc
{
    [TestFixture]
    public class NetqRpcRabbitClientTests
    {
        private Mock<INetqRabbitRpcConnection<Func<Type, object>>> _netqRpcRabbitConnectionMock;
        private Mock<ILogger> _logger;
        private Mock<IBus> _busMock;

        private NetqRabbitRpcClient _rpcClient;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            _netqRpcRabbitConnectionMock = new Mock<INetqRabbitRpcConnection<Func<Type, object>>>();
            _busMock = new Mock<IBus>();
            _netqRpcRabbitConnectionMock.Setup(connection => connection.Bus).Returns(_busMock.Object);

            Mock<ILoggerFactory> lfMock = new Mock<ILoggerFactory>();
            lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);

            _rpcClient = new NetqRabbitRpcClient(_netqRpcRabbitConnectionMock.Object, lfMock.Object);
        }

        [Test]
        public void Request_OnRequestAndAbstractMessageTypeBus_ShouldReturnBusRespond()
        {
            //Arrange
            RequestStub request = new RequestStub();
            RespondStub expectedRespond = new RespondStub { Value = 6 };
            _busMock.Setup(bus => bus.Request<RequestStub, RespondStub>(It.IsAny<RequestStub>())).Returns(expectedRespond);
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            //Act
            var actualRespond = _rpcClient.Request<RequestStub, RespondStub>(request);

            //Assert
            Assert.AreEqual(expectedRespond, actualRespond);
            _busMock.Verify(bus => bus.Request<RequestStub, RespondStub>(request), Times.Once);
        }

        [Test]
        public void Request_OnRequestAndConcreteMessageTypeBus_ShouldReturnBusRespond()
        {
            //Arrange
            RequestStub request = new RequestStub();
            RespondStub expectedRespond = new RespondStub { Value = 6 };
            _netqRpcRabbitConnectionMock.Setup(connection => connection.Request(It.IsAny<RequestStub>())).Returns(type => expectedRespond);
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            var actualRespond = _rpcClient.Request<RequestStub, RespondStub>(request);

            //Assert
            Assert.AreEqual(expectedRespond, actualRespond);
            _netqRpcRabbitConnectionMock.Verify(connection => connection.Request(request), Times.Once);
        }

        [Test]
        public void RequestAsync_OnRequestAndAbstractMessageTypeBus_ShouldReturnBusRespond()
        {
            //Arrange
            RequestStub request = new RequestStub();
            RespondStub expectedRespond = new RespondStub { Value = 6 };
            _busMock.Setup(bus => bus.RequestAsync<RequestStub, RespondStub>(It.IsAny<RequestStub>())).Returns(Task.Factory.StartNew(()=> expectedRespond));
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            //Act
            var actualRespond = _rpcClient.RequestAsync<RequestStub, RespondStub>(request).Result;

            //Assert
            Assert.AreEqual(expectedRespond, actualRespond);
            _busMock.Verify(bus => bus.RequestAsync<RequestStub, RespondStub>(request), Times.Once);
        }

        [Test]
        public void RequestAsync_OnRequestAndConcreteMessageTypeBus_ShouldReturnBusRespond()
        {
            //Arrange
            RequestStub request = new RequestStub();
            RespondStub expectedRespond = new RespondStub { Value = 6 };
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RequestAsync(It.IsAny<RequestStub>())).Returns(Task.Factory.StartNew(()=> new Func<Type,object>(type => expectedRespond)));
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            var actualRespond = _rpcClient.RequestAsync<RequestStub, RespondStub>(request).Result;

            //Assert
            Assert.AreEqual(expectedRespond, actualRespond);
            _netqRpcRabbitConnectionMock.Verify(connection => connection.RequestAsync(request), Times.Once);
        }

        [Test]
        public void Request_OnNonValidRpcMessageTypeInNetqRpcRabbitConnection_ShouldThrowInvalidOperationException()
        {
            //Arrange
            RequestStub request = new RequestStub();
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns((RpcMessageType)10);

            //Act
            Assert.Throws<InvalidOperationException>(()=>_rpcClient.Request<RequestStub, RespondStub>(request));
        }

        [Test]
        public void RequestAsync_OnNonValidRpcMessageTypeInNetqRpcRabbitConnection_ShouldThrowAggregateException()
        {
            //Arrange
            RequestStub request = new RequestStub();
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns((RpcMessageType)10);

            //Act
            Assert.Throws<AggregateException>(() => _rpcClient.RequestAsync<RequestStub, RespondStub>(request).Wait());
        }
    }
}
