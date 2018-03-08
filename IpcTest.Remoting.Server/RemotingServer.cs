using IpcTest.Common;
using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;

namespace IpcTest.Remoting.Server
{
    public sealed class RemotingServer : IIpcAsyncServer
    {
        private class _Server : IIpcClient
        {
            private readonly RemotingServer server;

            public _Server(RemotingServer server)
            {
                this.server = server;
            }

            public void Send(string data)
            {
                this.server.OnReceived(new DataReceivedEventArgs(data));
            }
        }

        private readonly ManualResetEvent killer = new ManualResetEvent(false);

        private static readonly IServerChannelSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                var properties = new Hashtable();
                properties["portName"] = typeof(IIpcClient).Name;
                properties["exclusiveAddressUse"] = false;
                properties["authorizedGroup"] = "Everyone";

                var channel = new IpcChannel(properties, null, serverSinkProvider);

                try
                {
                    ChannelServices.RegisterChannel(channel, true);
                }
                catch
                {
                }

                var remoteObject = new RemoteObject(new _Server(this));

                RemotingServices.Marshal(remoteObject, typeof(RemoteObject).Name + ".rem");

                this.killer.WaitOne();

                RemotingServices.Disconnect(remoteObject);

                try
                {
                    ChannelServices.UnregisterChannel(channel);
                }
                catch
                {
                }
            });
        }

        public void Stop()
        {
            this.killer.Set();
        }

        void IDisposable.Dispose()
        {
            this.Stop();

            this.killer.Dispose();
        }

        private void OnReceived(DataReceivedEventArgs e)
        {
            var handler = this.Received;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}
