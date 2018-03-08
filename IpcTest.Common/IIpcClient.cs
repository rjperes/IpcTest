using System.Runtime.InteropServices;
using System.ServiceModel;

namespace IpcTest.Common
{
    [ServiceContract]
    [ComVisible(true)]
    public interface IIpcClient
    {
        [OperationContract(IsOneWay = true)]
        void Send(string data);
    }
}
