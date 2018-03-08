using IpcTest.Common;
using IpcTest.ServicedComponent.Server;
using System;

namespace IpcTest.ServicedComponent.Client
{
    public class ServicedComponentClient : IIpcClient
    {
        public void Send(string data)
        {
            var proxy = Activator.CreateInstance(Type.GetTypeFromProgID(ServicedComponentServer.ProgId)) as IIpcClient;

            proxy.Send(data);

            proxy = null;
        }
    }
}
