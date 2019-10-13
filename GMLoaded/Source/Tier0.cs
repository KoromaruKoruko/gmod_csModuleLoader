using System;
using System.Runtime.InteropServices;
using GMLoaded.Lua;

namespace GMLoaded.Source
{
    internal static class Tier0_lin
    {
        [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConColorMsg(ref Color Color, String Fmt, String Arg0);

        [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConDMsg(String Fmt, String Arg0);

        [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConLog(Int32 Lvl, String Fmt, String Arg0);

        [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConMsg(Int32 lvl, String Fmt, String Arg0);

        [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DevMsg(String Fmt, String Arg0);

        [DllImport("libtier0.so", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DevWarning(String Fmt, String Arg0);
    }

    internal static class Tier0_win
    {
        [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConColorMsg(ref Color Color, String Fmt, String Arg0);

        [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConDMsg(String Fmt, String Arg0);

        [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConLog(Int32 Lvl, String Fmt, String Arg0);

        [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ConMsg(Int32 lvl, String Fmt, String Arg0);

        [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DevMsg(String Fmt, String Arg0);

        [DllImport("tier0.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DevWarning(String Fmt, String Arg0);
    }

    // im forced to do the ugly here, cant marshal generic delegates,
    [StaticInterface(typeof(Tier0_win), typeof(Tier0_lin))]
    public static class Tier0
    {
        public static void ConColorMsg(ref Color Color, String Fmt, String Arg0) => throw new NotLoadedException();

        public static void ConDMsg(String Fmt, String Arg0) => throw new NotLoadedException();

        public static void ConLog(Int32 Lvl, String Fmt, String Arg0) => throw new NotLoadedException();

        public static void ConMsg(Int32 Lvl, String Fmt, String Arg0) => throw new NotLoadedException();

        public static void DevMsg(String Fmt, String Arg0) => throw new NotLoadedException();

        public static void DevWarning(String Fmt, String Arg0) => throw new NotLoadedException();
    }
}
