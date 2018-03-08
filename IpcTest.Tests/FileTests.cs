using IpcTest.File.Client;
using IpcTest.File.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class FileTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<FileClient, FileServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
