using System;
using System.Runtime.InteropServices;
using GMLoaded.Native;
using GMLoaded;
using GMLoaded.Lua;

namespace ModuleExample
{
    public static class ModuleExportInterface
    {
        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Close32([MarshalAs(UnmanagedType.I4)]Int32 Ptr) => Module.Close(GLua.Get(Marshal.PtrToStructure<Lua_State32>(new IntPtr(Ptr))));

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Close64([MarshalAs(UnmanagedType.I8)]Int64 Ptr) => Module.Close(GLua.Get(Marshal.PtrToStructure<Lua_State64>(new IntPtr(Ptr))));

        public static void Define(
            [MarshalAs(UnmanagedType.Bool)] Boolean isX64,
            [MarshalAs(UnmanagedType.U1)] Byte Syscode
            ) => Natives.Init(isX64, (GMLoaded.System)Syscode);

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Open32([MarshalAs(UnmanagedType.I4)]Int32 Ptr) => Module.Open(GLua.Get(Marshal.PtrToStructure<Lua_State32>(new IntPtr(Ptr))));

        [return: MarshalAs(UnmanagedType.I4)]
        public static Int32 Open64([MarshalAs(UnmanagedType.I8)]Int64 Ptr) => Module.Open(GLua.Get(Marshal.PtrToStructure<Lua_State64>(new IntPtr(Ptr))));
    }
}
