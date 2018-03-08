using IpcTest.Com.Client;
using IpcTest.Com.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class ComTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<ComClient, ComServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
