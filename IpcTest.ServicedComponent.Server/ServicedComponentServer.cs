using IpcTest.Common;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ApplicationName("IpcClient")]
[assembly: AssemblyKeyFile("IpcTest.ServicedComponent.Server.snk")]
[assembly: ApplicationActivation(ActivationOption.Server)]
[assembly: ApplicationAccessControl(false)]

namespace IpcTest.ServicedComponent.Server
{
    public class DummyIpcClient : IIpcClient
    {
        public void Send(string data)
        {
            data.ToString();
        }
    }


    [ComVisible(true)]
    [JustInTimeActivation(true)]
    [ComponentAccessControl(false)]
    [ProgId(ServicedComponentServer.ProgId)]
    [ObjectPooling(Enabled = true, MaxPoolSize = 1, MinPoolSize = 1, CreationTimeout = 5000)]
    public sealed class ServicedComponentServer : System.EnterpriseServices.ServicedComponent, IIpcClient
    {
        public const string ProgId = "IpcTest.ServicedComponentServer";

        private readonly HashSet<IIpcClient> subscribers = new HashSet<IIpcClient>();

        protected override void Activate()
        {
        }

        protected override void Deactivate()
        {
        }

        protected override void Construct(string s)
        {
        }

        protected override bool CanBePooled()
        {
            return true;
        }

        [AutoComplete]
        public void Send(string data)
        {
            foreach (var client in this.subscribers)
            {
                client.Send(data);
            }
        }

        [AutoComplete]
        public void Register(IntPtr ptr)
        {
            var client = Marshal.PtrToStructure<DummyIpcClient>(ptr);

            this.subscribers.Add(client);
        }
    }
}
