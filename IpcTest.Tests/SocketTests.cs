using IpcTest.Socket.Client;
using IpcTest.Socket.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class SocketTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<SocketClient, SocketServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
