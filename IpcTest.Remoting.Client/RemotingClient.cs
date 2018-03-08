using IpcTest.Common;
using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;

namespace IpcTest.Remoting.Client
{
    public class RemotingClient : IIpcClient
    {
        private static readonly IServerChannelSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

        public void Send(string data)
        {
            try
            {
                var properties = new Hashtable();
                properties["portName"] = Guid.NewGuid().ToString();
                properties["exclusiveAddressUse"] = false;
                properties["authorizedGroup"] = "Everyone";

                var channel = new IpcChannel(properties, null, serverSinkProvider);

                try
                {
                    ChannelServices.RegisterChannel(channel, true);
                }
                catch
                {
                }

                var uri = string.Format("ipc://{0}/{1}.rem", typeof(IIpcClient).Name, typeof(RemoteObject).Name);
                var svc = Activator.GetObject(typeof(RemoteObject), uri) as IIpcClient;

                svc.Send(data);

                try
                {
                    ChannelServices.UnregisterChannel(channel);
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
