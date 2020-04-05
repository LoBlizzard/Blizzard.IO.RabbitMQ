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

    public class NetqRabbitRpcServerTests
    {
        private NetqRabbitRpcServer _server;
        private Mock<INetqRpcRabbitConnection<Func<Type, object>>> _netqRpcRabbitConnectionMock;
        private Mock<IBus> _busMock;

        [SetUp]
        public void Setup()
        {
            _busMock = new Mock<IBus>();
            _netqRpcRabbitConnectionMock = new Mock<INetqRpcRabbitConnection<Func<Type, object>>>();

            _netqRpcRabbitConnectionMock.Setup(connection => connection.Bus).Returns(_busMock.Object);
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            Mock<ILoggerFactory> lfMock = new Mock<ILoggerFactory>();
            lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

            _server = new NetqRabbitRpcServer(_netqRpcRabbitConnectionMock.Object, lfMock.Object);
        }

        [Test]
        public void Respond_OnValidCallbackAndConcreteRpcType_ShouldStartRespond()
        {
            //Arrange
            Func<RequestStub, RespondStub> callback = request => new RespondStub();
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            _server.Respond<RequestStub, RespondStub>(callback);

            //Assert
            _busMock.Verify(bus => bus.Respond<Func<Type, object>, RespondStub>(It.IsAny<Func<Func<Type, object>, RespondStub>>()), Times.Once);
        }

        [Test]
        public void Respond_OnValidCallbackAndAbstractRpcType_ShouldStartRespond()
        {
            //Arrange
            Func<RequestStub, RespondStub> callback = request => new RespondStub();
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            //Act
            _server.Respond<RequestStub, RespondStub>(callback);

            //Assert
            _busMock.Verify(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>()), Times.Once);
        }

        [Test]
        public void Respond_OnInValidRpcType_ShouldThrowInvalidOpertionException()
        {
            //Arrange
            Func<RequestStub, RespondStub> callback = request => new RespondStub();
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns((RpcMessageType)10);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.Respond<RequestStub, RespondStub>(callback));
        }

        [Test]
        public void Respond_OnRespondCalledMoreThanOneTime_ShouldThrowInvalidOpertionException()
        {
            //Arrange
            Func<RequestStub, RespondStub> callback = request => new RespondStub();
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>())).Returns(handlerMock.Object);
            _server.Respond(callback);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.Respond<RequestStub, RespondStub>(callback));
        }

        [Test]
        public void RespondAsync_OnValidCallbackAndConcreteRpcType_ShouldStartRespond()
        {
            //Arrange
            Func<RequestStub, Task<RespondStub>> callback = request => Task.Factory.StartNew(() => new RespondStub());
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Concrete);

            //Act
            _server.RespondAsync<RequestStub, RespondStub>(callback);

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
            _server.RespondAsync<RequestStub, RespondStub>(callback);

            //Assert
            _busMock.Verify(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>()), Times.Once);
        }

        [Test]
        public void RespondAsync_OnInValidRpcType_ShouldThrowInvalidOpertionException()
        {
            //Arrange
            Func<RequestStub, Task<RespondStub>> callback = request => Task.Factory.StartNew(() => new RespondStub());
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns((RpcMessageType)10);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.RespondAsync<RequestStub, RespondStub>(callback));
        }

        [Test]
        public void RespondAsync_OnRespondCalledMoreThanOneTime_ShouldThrowInvalidOpertionException()
        {
            //Arrange
            Func<RequestStub, Task<RespondStub>> callback = request => Task.Factory.StartNew(() => new RespondStub());
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);

            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>())).Returns(handlerMock.Object);
            _server.RespondAsync(callback);

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => _server.RespondAsync<RequestStub, RespondStub>(callback));
        }

        [Test]
        public void Stop_OnCallingAfterCallingRespond_ShouldStopResponding()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);
            Func<RequestStub, RespondStub> callback = request => new RespondStub();
            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>())).Returns(handlerMock.Object);
            _server.Respond(callback);

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
            Func<RequestStub, Task<RespondStub>> callback = request => Task.Factory.StartNew(() => new RespondStub());
            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>())).Returns(handlerMock.Object);
            _server.RespondAsync(callback);

            //Act
            _server.Stop();

            //Assert
            handlerMock.Verify(handler => handler.Dispose(), Times.Once);
        }

        [Test]
        public void Stop_OnCallingCallingRespondAsyncOrRespond_ShouldNotDisposeTheHandler()
        {
            //Arrange
            _netqRpcRabbitConnectionMock.Setup(connection => connection.RpcMessageType).Returns(RpcMessageType.Abstract);
            Func<RequestStub, Task<RespondStub>> callback = request => Task.Factory.StartNew(() => new RespondStub());
            Mock<IDisposable> handlerMock = new Mock<IDisposable>();
            _busMock.Setup(bus => bus.RespondAsync<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, Task<RespondStub>>>())).Returns(handlerMock.Object);
            _busMock.Setup(bus => bus.Respond<RequestStub, RespondStub>(It.IsAny<Func<RequestStub, RespondStub>>())).Returns(handlerMock.Object);

            //Act
            _server.Stop();

            //Assert
            handlerMock.Verify(handler => handler.Dispose(), Times.Never);
        }
    }
}
