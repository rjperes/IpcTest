using IpcTest.Wcf.Client;
using IpcTest.Wcf.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class WcfTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<WcfClient, WcfServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
