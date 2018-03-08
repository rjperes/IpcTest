using System.Runtime.InteropServices;
using IpcTest.ServicedComponent.Server;
using NUnit.Framework;

namespace IpcTest.Tests
{
    [TestFixture]
    public class ServicedComponentTests
    {

        [Test]
        public void CanSendAndReceive()
        {
            var client = new DummyIpcClient();
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<DummyIpcClient>());

            Marshal.StructureToPtr(client, ptr, false);

            var proxy = new ServicedComponentServer();
            proxy.Register(ptr);
            proxy.Send("Hello, World!");
        }
    }
}
