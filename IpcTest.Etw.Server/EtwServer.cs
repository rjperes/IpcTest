using IpcTest.Common;
using IpcTest.Etw.Client;
using System;
using System.Diagnostics.Tracing;

namespace IpcTest.Etw.Server
{
    public sealed class EtwServer : EventListener, IIpcServer
    {
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventName == typeof (IIpcClient).Name)
            {
                var data = eventData.Payload[0];
                this.OnReceived(new DataReceivedEventArgs(data.ToString()));
            }
        }

        public void Start()
        {
            this.EnableEvents(EtwClient.Instance, EventLevel.LogAlways);
        }

        public void Stop()
        {
            this.DisableEvents(EtwClient.Instance);
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
