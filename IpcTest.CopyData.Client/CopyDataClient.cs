using IpcTest.Common;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IpcTest.CopyData.Client
{
    public class CopyDataClient : IIpcClient
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public IntPtr cbData;
            public IntPtr lpData;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private const int HwndBroadcast = 0xffff;

        private const int WM_COPY_DATA = 0x004A;

        public void Send(string data)
        {
            var cds = new COPYDATASTRUCT();
            cds.dwData = (IntPtr) Marshal.SizeOf(cds);
            cds.cbData = (IntPtr) data.Length;
            cds.lpData = Marshal.StringToHGlobalAnsi(data);

            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));

            Marshal.StructureToPtr(cds, ptr, true);

            var target = FindWindow(null, typeof(IIpcClient).Name);  //(IntPtr)HwndBroadcast;

            var result = SendMessage(target, WM_COPY_DATA, IntPtr.Zero, ptr);

            Marshal.FreeHGlobal(cds.lpData);
            Marshal.FreeCoTaskMem(ptr);
        }
    }
}
