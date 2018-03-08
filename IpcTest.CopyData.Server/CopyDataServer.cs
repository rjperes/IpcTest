using IpcTest.Common;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IpcTest.CopyData.Server
{
    public class CopyDataServer : IIpcServer
    {
        private NativeWindow messageHandler;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public IntPtr cbData;
            public IntPtr lpData;
        }

        private const int WM_COPY_DATA = 0x004A;
        private const int WM_QUIT = 0x0012;

        private sealed class MessageHandler : NativeWindow
        {
            private readonly CopyDataServer server;

            public MessageHandler(CopyDataServer server)
            {
                this.server = server;
                this.CreateHandle(new CreateParams() { Caption = typeof(IIpcClient).Name });
            }

            protected override void WndProc(ref Message msg)
            {
                if (msg.Msg == WM_COPY_DATA)
                {
                    var cds = (COPYDATASTRUCT) Marshal.PtrToStructure(msg.LParam, typeof(COPYDATASTRUCT));

                    if (cds.cbData.ToInt32() > 0)
                    {
                        var bytes = new byte[cds.cbData.ToInt32()];

                        Marshal.Copy(cds.lpData, bytes, 0, cds.cbData.ToInt32());

                        var chars = Encoding.ASCII.GetChars(bytes);
                        var data = new string(chars);

                        this.server.OnReceived(new DataReceivedEventArgs(data));
                    }

                    msg.Result = (IntPtr) 1;
                }

                base.WndProc(ref msg);
            }
        }

        private void OnReceived(DataReceivedEventArgs e)
        {
            var handler = this.Received;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        void IDisposable.Dispose()
        {
            this.Stop();
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                this.messageHandler = new MessageHandler(this);

                Application.Run();
            });

            //Thread.Sleep(2 * 1000);
        }

        public void Stop()
        {
            SendMessage(this.messageHandler.Handle, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
        }

        public event EventHandler<DataReceivedEventArgs> Received;
    }
}
