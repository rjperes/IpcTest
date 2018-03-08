using IpcTest.Common;
using System;
using System.ServiceModel;

namespace IpcTest.Wcf.Server
{
    public sealed class WcfServer : IIpcServer
    {
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        private class _Server : IIpcClient
        {
            private readonly WcfServer server;

            public _Server(WcfServer server)
            {
                this.server = server;
            }

            public void Send(string data)
            {
                this.server.OnReceived(new DataReceivedEventArgs(data));
            }
        }

        private readonly ServiceHost host;

        private void OnReceived(DataReceivedEventArgs e)
        {
            var handler = this.Received;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public WcfServer()
        {
            this.host = new ServiceHost(new _Server(this), new Uri(string.Format("net.pipe://localhost/{0}", typeof(IIpcClient).Name)));
        }

        public event EventHandler<DataReceivedEventArgs> Received;

        public void Start()
        {
            this.host.Open();
        }

        public void Stop()
        {
            this.host.Close();
        }

        void IDisposable.Dispose()
        {
            this.Stop();

            (this.host as IDisposable).Dispose();
        }
    }
}
