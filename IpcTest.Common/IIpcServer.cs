using System;
using System.Runtime.InteropServices;

namespace IpcTest.Common
{
    [ComVisible(true)]
    public interface IIpcServer : IDisposable
    {
        void Start();
        void Stop();

        event EventHandler<DataReceivedEventArgs> Received;
    }
}
