using IpcTest.Common;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IpcTest.File.Server
{
    public sealed class FileServer : IIpcAsyncServer
    {
        private const string filename = "Filename.txt";
        private const int delay = 100;
        private readonly ManualResetEvent killer = new ManualResetEvent(false);

        void IDisposable.Dispose()
        {
            this.Stop();

            this.killer.Dispose();
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                while (this.killer.WaitOne(0) == false)
                {
                    try
                    {
						if (new FileInfo(filename).Length > 0)
						{						
							var file = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);

							using (var reader = new StreamReader(file))
							{
								var data = reader.ReadToEnd();

								this.OnReceive(new DataReceivedEventArgs(data));

								file.Close();
							}
						}
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(delay);
                    }
                }
            });
        }

        public void Stop()
        {
            this.killer.Set();
        }

        private void OnReceive(DataReceivedEventArgs e)
        {
            var handler = this.Received;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}
