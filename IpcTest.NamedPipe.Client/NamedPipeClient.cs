using IpcTest.Common;
using System.IO;
using System.IO.Pipes;

namespace IpcTest.NamedPipe.Client
{
    public class NamedPipeClient : IIpcClient
    {
        public void Send(string data)
        {
            using (var client = new NamedPipeClientStream(".", typeof(IIpcClient).Name, PipeDirection.Out))
            {
                client.Connect();

                using (var writer = new StreamWriter(client))
                {
                    writer.WriteLine(data);
                }
            }
        }
    }
}
