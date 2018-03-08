using IpcTest.Com.Server;
using IpcTest.Common;
using System;

namespace IpcTest.Com.Client
{
    public class ComClient : IIpcClient
    {
        public void Send(string data)
        {
            var proxy = Activator.CreateInstance(Type.GetTypeFromProgID(IpcClientServer.ProgId)) as IIpcClientServer;

            proxy.Send(data);

            proxy = null;
        }
    }
}
