using IpcTest.Common;
using System.IO;
using System.Threading;

namespace IpcTest.File.Client
{
    public class FileClient : IIpcClient
    {
        private const string filename = "Filename.txt";
        private const int delay = 100;

        public void Send(string data)
        {
            while (true)
            {
                try
                {
                    var file = System.IO.File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None);

                    using (var writer = new StreamWriter(file))
                    {
                        writer.Write(data);
                        writer.Flush();
                    }

                    file.Close();

                    break;
                }
                catch (IOException)
                {
                    Thread.Sleep(delay);
                }
            }
        }
    }
}
