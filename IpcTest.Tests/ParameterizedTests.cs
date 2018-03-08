using IpcTest.Common;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace IpcTest.Tests
{
    public class ParameterizedTests<TClient, TServer> where TClient : IIpcClient, new() where TServer : IIpcServer, new()
    {
        private readonly IIpcClient client;
        private readonly IIpcServer server;

        public ParameterizedTests() : this(new TClient(), new TServer())
        {
        }

        public ParameterizedTests(IIpcClient client) : this(client, new TServer())
        {
        }

        public ParameterizedTests(IIpcServer server) : this(new TClient(), server)
        {
        }

        public ParameterizedTests(IIpcClient client, IIpcServer server)
        {
            this.client = client;
            this.server = server;
        }

        protected IIpcClient GetService()
        {
            return this.client;
        }

        protected IIpcServer GetReceiver()
        {
            return this.server;
        }

        public void CanSendAndReceive()
        {
            using (var canStart = new ManualResetEvent(false))
            using (var canFinish = new ManualResetEvent(false))
            {
                var text = "Hello, World!";

                var client = Task.Factory.StartNew(() =>
                {
                    canStart.WaitOne();

                    var service = this.GetService();
                    service.Send(text);
                });

                var server = Task.Factory.StartNew(() =>
                {
                    using (var receiver = this.GetReceiver())
                    {
                        receiver.Received += (s, e) =>
                        {
                            Assert.AreEqual(text, e.Data);
                            canFinish.Set();
                        };

                        receiver.Start();

                        canStart.Set();

                        var result = canFinish.WaitOne();

                        Assert.IsTrue(result);
                    }
                });

                Task.WaitAll(client, server);

                Assert.IsNull(client.Exception);
                Assert.IsNull(server.Exception);
            }
        }
    }
}
