using System.Runtime.InteropServices;
using System;

namespace GSharp.Generated.NativeClasses
{
    public struct Lua_State
    {
        public Lua_State(Byte[] _ignore_this_common_lua_header_, IntPtr luabase)
        {
            this._ignore_this_common_lua_header_ = _ignore_this_common_lua_header_;
            this.Luabase = luabase;
        }
        public Byte[] _ignore_this_common_lua_header_;
        public IntPtr Luabase;

        public static implicit operator Lua_State(Lua_State32 x32) => new Lua_State(x32._ignore_this_common_lua_header_, x32.Luabase);
        public static implicit operator Lua_State(Lua_State64 x64) => new Lua_State(x64._ignore_this_common_lua_header_, x64.Luabase);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Lua_State32
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 70)]
        public Byte[] _ignore_this_common_lua_header_;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 LuabaseP;
        public IntPtr Luabase => new IntPtr(this.LuabaseP);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Lua_State64
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 114)]
        public Byte[] _ignore_this_common_lua_header_;
        [MarshalAs(UnmanagedType.I8)]
        public Int64 LuabaseP;
        public IntPtr Luabase => new IntPtr(this.LuabaseP);
    }
}
