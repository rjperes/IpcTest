using IpcTest.MessageQueue.Client;
using IpcTest.MessageQueue.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class MessageQueueTests
    {
        [Test]
        public void CanSendAndReceive()
        {
            var test = new ParameterizedTests<MessageQueueClient, MessageQueueServer>();
            Assert.DoesNotThrow(() => test.CanSendAndReceive());
        }
    }
}
