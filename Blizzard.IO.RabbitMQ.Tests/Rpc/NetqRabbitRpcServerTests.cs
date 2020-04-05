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
    public class NetqRabbitRpcServerTests
    {
        private Mock<INetqRpcRabbitConnection<Func<Type, object>>> _netqRpcRabbitConnectionMock;
        private Mock<IBus> _busMock;
        
        private NetqRabbitRpcServer _server;

        [SetUp]
        public void Setup()
        {
            _busMock = new Mock<IBus>();
            _netqRpcRabbitConnectionMock = new Mock<INetqRpcRabbitConnection<Func<Type, object>>>();

            _netqRpcRabbitConnectionMock.Setup(connection => connection.Bus).Returns(_busMock.Object);

            //Setting up the logger
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ILoggerFactory> lfMock = new Mock<ILoggerFactory>();
            lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

            _server = new NetqRabbitRpcServer(_netqRpcRabbitConnectionMock.Object, lfMock.Object);
        }

        [Test]
        public void Respond_OnValidCallbackAndConcreteRpcType_ShouldStartRespond()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            _server.Respond<RequestStub, RespondStub>(Callback);

            //Assert
            _busMock.Verify(bus => bus.Respond<Func<Type, object>, RespondStub>(It.IsAny<Func<Func<Type, object>, RespondStub>>()), Times.Once);
        }

        [Test]
        public void Respond_OnValidCallbackAndAbstractRpcType_ShouldStartRespond()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            //Act
            _server.Respond<RequestStub, RespondStub>(Callback);

            //Assert
            _busMock.Verify(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>()), Times.Once);
        }

        [Test]
        public void Respond_OnInValidRpcType_ShouldThrowInvalidOperationException()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns((RpcMessageType)10);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.Respond<RequestStub, RespondStub>(Callback));
        }

        [Test]
        public void Respond_OnRespondCalledMoreThanOneTime_ShouldThrowInvalidOperationException()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>())).Returns(handlerMock.Object);
            _server.Respond<RequestStub, RespondStub>(Callback);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.Respond<RequestStub, RespondStub>(Callback));
        }

        [Test]
        public void RespondAsync_OnValidCallbackAndConcreteRpcType_ShouldStartRespond()
        {
            //Arrange
            Func<RequestStub, Task<RespondStub>> callback = request => Task.Factory.StartNew(() => new RespondStub());
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            _server.RespondAsync<RequestStub, RespondStub>(AsyncCallback);

            //Assert
            _busMock.Verify(bus => bus.RespondAsync<Func<Type, object>, RespondStub>(It.IsAny<Func<Func<Type, object>, Task<RespondStub>>>()), Times.Once);
        }

        [Test]
        public void RespondAsync_OnValidCallbackAndAbstractRpcType_ShouldStartRespond()
        {
            //Arrange
            Func<RequestStub, Task<RespondStub>> callback = request => Task.Factory.StartNew(() => new RespondStub());
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            //Act
            _server.RespondAsync<RequestStub, RespondStub>(AsyncCallback);

            //Assert
            _busMock.Verify(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>()), Times.Once);
        }

        [Test]
        public void RespondAsync_OnInValidRpcType_ShouldThrowInvalidOperationException()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns((RpcMessageType)10);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.RespondAsync<RequestStub, RespondStub>(AsyncCallback));
        }

        [Test]
        public void RespondAsync_OnRespondCalledMoreThanOneTime_ShouldThrowInvalidOperationException()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);
            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>())).Returns(handlerMock.Object);
            _server.RespondAsync<RequestStub, RespondStub>(AsyncCallback);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.RespondAsync<RequestStub, RespondStub>(AsyncCallback));
        }

        [Test]
        public void Stop_OnCallingAfterCallingRespond_ShouldStopResponding()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);
            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>())).Returns(handlerMock.Object);
            _server.Respond<RequestStub, RespondStub>(Callback);

            //Act
            _server.Stop();

            //Assert
            handlerMock.Verify(handler => handler.Dispose(), Times.Once);
        }


        [Test]
        public void Stop_OnCallingAfterCallingRespondAsync_ShouldStopResponding()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);
            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>())).Returns(handlerMock.Object);
            _server.RespondAsync<RequestStub, RespondStub>(AsyncCallback);

            //Act
            _server.Stop();

            //Assert
            handlerMock.Verify(handler => handler.Dispose(), Times.Once);
        }

        [Test]
        public void Stop_OnCallingRespondAsyncOrRespond_ShouldNotDisposeTheHandler()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);
            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>())).Returns(handlerMock.Object);
            _busMock.Setup(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>())).Returns(handlerMock.Object);

            //Act
            _server.Stop();

            //Assert
            handlerMock.Verify(handler => handler.Dispose(), Times.Never);
        }

        private RespondStub Callback(RequestStub request) => new RespondStub();

        private Task<RespondStub> AsyncCallback(RequestStub request) => Task.Factory.StartNew(() => new RespondStub());
    }
}
