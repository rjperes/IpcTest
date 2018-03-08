using IpcTest.Common;
using System.Net.Sockets;
using System.Text;

namespace IpcTest.Socket.Client
{
    public class SocketClient : IIpcClient
    {
        public void Send(string data)
        {
            using (var client = new UdpClient())
            {
                client.Connect(string.Empty, 9000);

                var bytes = Encoding.Default.GetBytes(data);

                client.Send(bytes, bytes.Length);
            }
        }
    }
}
