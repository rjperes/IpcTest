using IpcTest.SharedMemory.Client;
using IpcTest.SharedMemory.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class SharedMemoryTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<SharedMemoryClient, SharedMemoryServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
