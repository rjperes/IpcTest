using IpcTest.Common;
using System.Diagnostics.Tracing;

namespace IpcTest.Etw.Client
{
    //[EventSource(Name = EtwClient.EventName, Guid = EtwClient.EventGuid)]
    public class EtwClient : EventSource, IIpcClient
    {
        //public const string EventGuid = "406bb73a-d9bc-41b1-b3bc-1be071b26c9a";
        //public const string EventName = "IIpcClient";
        public static readonly string EventName = typeof(IIpcClient).Name;

        public static readonly EtwClient Instance = new EtwClient();

        private EtwClient(): base(EventName)
        {
        }

        //[Event(1, Message = "Sending {0}")]
        public void Send(string data)
        {
            this.Write(EventName, new { Data = data });
        }
    }
}
