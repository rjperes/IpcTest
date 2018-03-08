using IpcTest.Remoting.Client;
using IpcTest.Remoting.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class RemotingTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<RemotingClient, RemotingServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
