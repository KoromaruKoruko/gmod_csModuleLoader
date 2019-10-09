using GSharp.Native.Classes;

using System;
using System.Runtime.InteropServices;

namespace GSharp.Native
{
    /// <summary>
    /// Interface Class for the Source Tier0.dll
    /// </summary>
    public static class Tier0
    {
        const String DllName = "tier0.dll";
        const CallingConvention CConv = CallingConvention.Cdecl;

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConColorMsg(Int32 Lvl, ref Color Clr, String Fmt, String Arg0);

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConMsg(Int32 Lvl, String Fmt, String Arg0);

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConWarning(Int32 Lvl, String Fmt, String Arg0);

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConLog(Int32 Lvl, String Fmt, String Arg0);

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConDColorMsg(Int32 Lvl, ref Color Clr, String Fmt, String Arg0);

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConDMsg(Int32 Lvl, String Fmt, String Arg0);

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConDWarning(Int32 Lvl, String Fmt, String Arg0);

        [DllImport(DllName, CallingConvention = CConv)]
        static extern void ConDLog(Int32 Lvl, String Fmt, String Arg0);

        public static void ConColorMsg(Int32 Lvl, ref Color Clr, String Str) => ConColorMsg(Lvl, ref Clr, "%s", Str);

        public static void ConMsg(Int32 Lvl, String Str) => ConMsg(Lvl, "%s", Str);

        public static void ConWarning(Int32 Lvl, String Str) => ConWarning(Lvl, "%s", Str);

        public static void ConLog(Int32 Lvl, String Str) => ConLog(Lvl, "%s", Str);

        public static void ConDColorMsg(Int32 Lvl, ref Color Clr, String Str) => ConDColorMsg(Lvl, ref Clr, "%s", Str);

        public static void ConDMsg(Int32 Lvl, String Str) => ConDMsg(Lvl, "%s", Str);

        public static void ConDWarning(Int32 Lvl, String Str) => ConDWarning(Lvl, "%s", Str);

        public static void ConDLog(Int32 Lvl, String Str) => ConDLog(Lvl, "%s", Str);
    }
}
