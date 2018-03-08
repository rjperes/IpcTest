using IpcTest.CopyData.Client;
using IpcTest.CopyData.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class CopyDataTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<CopyDataClient, CopyDataServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
