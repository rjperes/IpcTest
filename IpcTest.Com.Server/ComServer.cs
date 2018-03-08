using IpcTest.Common;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace IpcTest.Com.Server
{
    static internal class COMHelper
    {
        public static void RegasmRegisterLocalServer(Type type)
        {
            using (var keyCLSID = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + type.GUID.ToString("B"), true))
            {
                keyCLSID.DeleteSubKeyTree("InprocServer32");

                using (var subkey = keyCLSID.CreateSubKey("LocalServer32"))
                {
                    subkey.SetValue(string.Empty, typeof(IIpcClientServer).Assembly.Location, RegistryValueKind.String);
                }
            }
        }

        public static void RegasmUnregisterLocalServer(Type type)
        {
            Registry.ClassesRoot.DeleteSubKeyTree(@"CLSID\" + type.GUID.ToString("B"));
        }
    }

    static internal class COMNative
    {
        [DllImport("ole32.dll")]
        public static extern int CoRegisterClassObject(ref Guid rclsid, [MarshalAs(UnmanagedType.Interface)] IClassFactory pUnk, CLSCTX dwClsContext, REGCLS flags, out uint lpdwRegister);

        [DllImport("ole32.dll")]
        public static extern uint CoRevokeClassObject(uint dwRegister);

        [DllImport("ole32.dll")]
        public static extern int CoResumeClassObjects();

        public const string IID_IClassFactory = "00000001-0000-0000-C000-000000000046";
        public const string IID_IUnknown = "00000000-0000-0000-C000-000000000046";
        public const string IID_IDispatch = "00020400-0000-0000-C000-000000000046";

        public const int CLASS_E_NOAGGREGATION = unchecked((int)0x80040110);
        public const int E_NOINTERFACE = unchecked((int)0x80004002);

        public const int S_OK = 0;
    }

    [ComImport]
    [ComVisible(false)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(COMNative.IID_IClassFactory)]
    internal interface IClassFactory
    {
        [PreserveSig]
        int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject);

        [PreserveSig]
        int LockServer(bool fLock);
    }

    [Flags]
    internal enum CLSCTX : uint
    {
        INPROC_SERVER = 0x1,
        INPROC_HANDLER = 0x2,
        LOCAL_SERVER = 0x4,
        INPROC_SERVER16 = 0x8,
        REMOTE_SERVER = 0x10,
        INPROC_HANDLER16 = 0x20,
        RESERVED1 = 0x40,
        RESERVED2 = 0x80,
        RESERVED3 = 0x100,
        RESERVED4 = 0x200,
        NO_CODE_DOWNLOAD = 0x400,
        RESERVED5 = 0x800,
        NO_CUSTOM_MARSHAL = 0x1000,
        ENABLE_CODE_DOWNLOAD = 0x2000,
        NO_FAILURE_LOG = 0x4000,
        DISABLE_AAA = 0x8000,
        ENABLE_AAA = 0x10000,
        FROM_DEFAULT_CONTEXT = 0x20000,
        ACTIVATE_32_BIT_SERVER = 0x40000,
        ACTIVATE_64_BIT_SERVER = 0x80000,
        CLSCTX_ENABLE_CLOAKING = 0x100000,
        CLSCTX_APPCONTAINER = 0x400000,
        CLSCTX_ACTIVATE_AAA_AS_IU = 0x800000,
        CLSCTX_PS_DLL = 0x80000000
    }

    [Flags]
    internal enum REGCLS : uint
    {
        SINGLEUSE = 0,
        MULTIPLEUSE = 1,
        MULTI_SEPARATE = 2,
        SUSPENDED = 4,
        SURROGATE = 8,
    }

    [ComVisible(true)]
    internal sealed class IpcClientServerClassFactory : IClassFactory
    {
        internal static IpcClientServer instance;

        private readonly ComServer server;

        public IpcClientServerClassFactory(ComServer server)
        {
            this.server = server;
        }

        public int CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;

            if (pUnkOuter != IntPtr.Zero)
            {
                Marshal.ThrowExceptionForHR(COMNative.CLASS_E_NOAGGREGATION);
            }

            if ((riid == new Guid(IpcClientServer.ClassId)) ||
                (riid == new Guid(COMNative.IID_IDispatch)) ||
                (riid == new Guid(COMNative.IID_IUnknown)))
            {
                if (instance == null)
                {
                    instance = new IpcClientServer();
                    instance.Received += (s, e) =>
                    {
                        this.server.OnReceived(e);
                    };
                }

                ppvObject = Marshal.GetComInterfaceForObject(instance, typeof(IIpcClientServer));
            }
            else
            {
                Marshal.ThrowExceptionForHR(COMNative.E_NOINTERFACE);
            }

            return COMNative.S_OK;
        }

        public int LockServer(bool fLock)
        {
            return COMNative.S_OK;
        }
    }

    [ComImport]
    [ComVisible(true)]
    [CoClass(typeof(IpcClientServer))]
    [Guid(IpcClientServer.InterfaceId)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IIpcClientServer
    {
        [DispId(1)]
        void Send(string data);
        [DispId(2)]
        event EventHandler<DataReceivedEventArgs> Received;
    }

    [ComVisible(true)]
    [ProgId(IpcClientServer.ProgId)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid(IpcClientServer.ClassId)]
    public sealed class IpcClientServer : IIpcClientServer
    {
        public const string ProgId = "IpcTest.IpcClientServer";
        public const string ClassId = "13FE32AD-4BF8-495f-AB4D-6C61BD463EA4";
        public const string InterfaceId = "D6F88E95-8A27-4ae6-B6DE-0542A0FC7039";

        [ComRegisterFunction]
        public static void Register(Type type)
        {
            COMHelper.RegasmRegisterLocalServer(type);
        }

        [ComUnregisterFunction]
        public static void Unregister(Type type)
        {
            COMHelper.RegasmUnregisterLocalServer(type);
        }

        public void Send(string data)
        {
            var handler = this.Received;

            if (handler != null)
            {
                handler(this, new DataReceivedEventArgs(data));
            }
        }

        public event EventHandler<DataReceivedEventArgs> Received;

        public static void Unregister(uint cookie)
        {
            if (cookie != 0)
            {
                COMNative.CoRevokeClassObject(cookie);
            }
        }

        public static uint Register(ComServer server)
        {
            var clsid = new Guid(IpcClientServer.ClassId);
            var result = (uint)COMNative.S_OK;
            var hResult = COMNative.CoRegisterClassObject(ref clsid, new IpcClientServerClassFactory(server), CLSCTX.LOCAL_SERVER, REGCLS.MULTIPLEUSE | REGCLS.SUSPENDED, out result);

            if (hResult != COMNative.S_OK)
            {
                throw new ApplicationException("CoRegisterClassObject failed w/err 0x" + hResult.ToString("X"));
            }

            hResult = COMNative.CoResumeClassObjects();

            if (hResult != COMNative.S_OK)
            {
                if (result != COMNative.S_OK)
                {
                    COMNative.CoRevokeClassObject(result);
                }

                throw new ApplicationException("CoResumeClassObjects failed w/err 0x" + hResult.ToString("X"));
            }

            return result;
        }
    }

    public sealed class ComServer : IIpcServer
    {
        private uint cookie;

        void IDisposable.Dispose()
        {
            this.Stop();
        }

        public void Start()
        {
            this.cookie = IpcClientServer.Register(this);
        }

        public void Stop()
        {
            IpcClientServer.Unregister(this.cookie);
        }

        internal void OnReceived(DataReceivedEventArgs e)
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
