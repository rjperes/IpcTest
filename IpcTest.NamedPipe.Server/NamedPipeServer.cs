using IpcTest.Common;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace IpcTest.NamedPipe.Server
{
    public sealed class NamedPipeServer : IIpcAsyncServer
    {
        private readonly NamedPipeServerStream server = new NamedPipeServerStream(typeof (IIpcClient).Name, PipeDirection.In);

        private void OnReceived(DataReceivedEventArgs e)
        {
            var handler = this.Received;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<DataReceivedEventArgs> Received;

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    this.server.WaitForConnection();

                    using (var reader = new StreamReader(this.server))
                    {
                        this.OnReceived(new DataReceivedEventArgs(reader.ReadToEnd()));
                    }
                }
            });
        }

        public void Stop()
        {
            this.server.Disconnect();
        }

        void IDisposable.Dispose()
        {
            this.Stop();

            this.server.Dispose();
        }
    }
}
