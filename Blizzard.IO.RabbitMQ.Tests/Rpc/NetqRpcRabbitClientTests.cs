using Blizzard.IO.Core.Rpc;
using Blizzard.IO.RabbitMQ.Entities.Rpc;
using Blizzard.IO.RabbitMQ.Rpc;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Blizzard.IO.RabbitMQ.Tests.Rpc
{
    class RequestStub
    {
        public int Value { get; set; }
    }

    class RespondStub
    {
        public int Value { get; set; }
    }

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

            _rpcClient = new NetqRabbitRpcClient(_netqRpcRabbitConnectionMock.Object, _logger.Object);
        }

        [Test]
        public void Request_OnValidRequestAndAbstractMessageTypeBus_ShouldReturnValidRespone()
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
        public void Request_OnValidRequestAndConcreteMessageTypeBus_ShouldReturnValidRespone()
        {
            //Arrange
            RequestStub request = new RequestStub();
            RespondStub expectedRespond = new RespondStub { Value = 6 };
            _busMock.Setup(bus => bus.Request<RequestStub, Func<Type, object>>(It.IsAny<RequestStub>())).Returns(type => expectedRespond);
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            var actualRespond = _rpcClient.Request<RequestStub, RespondStub>(request);

            //Assert
            Assert.AreEqual(expectedRespond, actualRespond);
            _busMock.Verify(bus => bus.Request<RequestStub, Func<Type, object>>(request), Times.Once);
        }

        [Test]
        public void RequestAsync_OnValidRequestAndAbstractMessageTypeBus_ShouldReturnValidRespone()
        {
            //Arrange
            RequestStub request = new RequestStub();
            RespondStub expectedRespond = new RespondStub { Value = 6 };
            _busMock.Setup(bus => bus.RequestAsync<RequestStub, RespondStub>(It.IsAny<RequestStub>())).Returns(Task.Factory.StartNew(()=> expectedRespond));
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            //Act
            var actualRespond = _rpcClient.RequestASync<RequestStub, RespondStub>(request).Result;

            //Assert
            Assert.AreEqual(expectedRespond, actualRespond);
            _busMock.Verify(bus => bus.RequestAsync<RequestStub, RespondStub>(request), Times.Once);
        }

        [Test]
        public void RequestAsync_OnValidRequestAndConcreteMessageTypeBus_ShouldReturnValidRespone()
        {
            //Arrange
            RequestStub request = new RequestStub();
            RespondStub expectedRespond = new RespondStub { Value = 6 };
            _busMock.Setup(bus => bus.RequestAsync<RequestStub, Func<Type, object>>(It.IsAny<RequestStub>())).Returns(Task.Factory.StartNew(()=> new Func<Type,object>(type => expectedRespond)));
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            var actualRespond = _rpcClient.RequestASync<RequestStub, RespondStub>(request).Result;

            //Assert
            Assert.AreEqual(expectedRespond, actualRespond);
            _busMock.Verify(bus => bus.RequestAsync<RequestStub, Func<Type, object>>(request), Times.Once);
        }
    }
}
