using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Libraria.Native
{
    public class NativeUtils
    {
        static readonly Dictionary<String, IntPtr> LibCache = new Dictionary<String, IntPtr>();

        public static IntPtr LoadLibrary(String Pth)
        {
            if (LibCache.ContainsKey(Pth)) // TODO: Handle extensions
                return LibCache[Pth];

            IntPtr Handle = Kernel32.GetModuleHandle(Pth);
            if (Handle == IntPtr.Zero)
                Handle = Kernel32.LoadLibrary(Pth);
            if (Handle == IntPtr.Zero)
                return IntPtr.Zero;

            LibCache.Add(Pth, Handle);
            return Handle;
        }

        public static IntPtr GetAddress(IntPtr Module, String Name) => Kernel32.GetProcAddress(Module, Name);

        public static IntPtr GetAddress<T>(IntPtr Module, String Name) where T : struct => GetAddress(Module, Name) - Marshal.SizeOf<T>();
    }
}