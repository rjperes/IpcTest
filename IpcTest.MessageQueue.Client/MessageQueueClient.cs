using System;
using IpcTest.Common;

namespace IpcTest.MessageQueue.Client
{
    public class MessageQueueClient : IIpcClient
    {
        public void Send(string data)
        {
            var name = string.Format(".\\Private$\\{0}", typeof(IIpcClient).Name);

            var queue = null as System.Messaging.MessageQueue;

            if (System.Messaging.MessageQueue.Exists(name) == true)
            {
                queue = new System.Messaging.MessageQueue(name);
            }
            else
            {
                queue = System.Messaging.MessageQueue.Create(name);
            }

            using (queue)
            {
                queue.Send(data);
            }
        }
    }
}
