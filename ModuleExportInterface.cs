using GSharp.Generated.NativeClasses;
using GSharp.GLuaNET;
using GSharp.GLuaNET.TypeMarshals;
using GSharp.Native.StringTable;
using GSharp.Native.Classes.VCR;
using GSharp.Native.Classes;
using GSharp.Native;
using System;
using System.Runtime.InteropServices;
using System.IO;

namespace SADmin
{
    public enum System : byte
    {
        Linux = 0,
        Windows = 1,
        OSx = 2,
    }

    public static class ModuleExportInterface
    {
        public static Boolean isX64;
        public static System System;
        public static string Tier0DllName;

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Close32([MarshalAs(UnmanagedType.I4)]Int32 Ptr) => Module.Close(GLua.Get(Marshal.PtrToStructure<Lua_State32>(new IntPtr(Ptr))));

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Close64([MarshalAs(UnmanagedType.I8)]Int64 Ptr) => Module.Close(GLua.Get(Marshal.PtrToStructure<Lua_State64>(new IntPtr(Ptr))));

        public static void Define(
            [MarshalAs(UnmanagedType.Bool)] Boolean isX64,
            [MarshalAs(UnmanagedType.U1)] Byte System)
        {
            ModuleExportInterface.System = (System)System;
            ModuleExportInterface.isX64 = isX64;

            switch (ModuleExportInterface.System)
            {
                case SADmin.System.Windows:
                    Tier0DllName = "tier0"; // safe to resolve like so
                    break;

                case SADmin.System.OSx:
                    break;

                case SADmin.System.Linux:
                    Tier0DllName = "libtier0.so";
                    break;
            }
        }

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Open32([MarshalAs(UnmanagedType.I4)]Int32 Ptr) => Module.Open(GLua.Get(Marshal.PtrToStructure<Lua_State32>(new IntPtr(Ptr))));

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Open64([MarshalAs(UnmanagedType.I8)]Int64 Ptr) => Module.Open(GLua.Get(Marshal.PtrToStructure<Lua_State64>(new IntPtr(Ptr))));
    }
}
