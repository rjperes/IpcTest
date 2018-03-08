using IpcTest.Common;
using System.ServiceModel;

namespace IpcTest.Wcf.Client
{
    public class WcfClient : ClientBase<IIpcClient>, IIpcClient
    {
        public WcfClient() : base(new NetNamedPipeBinding(), new EndpointAddress(string.Format("net.pipe://localhost/{0}", typeof(IIpcClient).Name)))
        {
        }

        public void Send(string data)
        {
            this.Channel.Send(data);
        }
    }
}
