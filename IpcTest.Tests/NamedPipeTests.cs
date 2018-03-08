using IpcTest.NamedPipe.Client;
using IpcTest.NamedPipe.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class NamedPipeTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<NamedPipeClient, NamedPipeServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
