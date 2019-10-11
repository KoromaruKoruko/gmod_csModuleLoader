using System;
using System.Runtime.InteropServices;

namespace GMLoaded.Lua
{
    /// <summary>
    /// LUA CFunc handle and/or loader
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Int32 CFunc(IntPtr L);

    public enum GarrysMod_Lua_State
    {
        CLIENT = 0,
        SERVER = 1,
        MENU = 2,
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate IntPtr CreateInterfaceFn(String pName, IntPtr pReturnCode);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void pfnStringChanged(IntPtr @object, IntPtr stringTable, Int32 stringNumber, String newString, IntPtr newData);

    public class LuaType
    {
        public static readonly LuaType None = new LuaType(-1, "None");
        public static readonly LuaType Nil = new LuaType(0, "Nil");
        public static readonly LuaType Boolean = new LuaType(1, "Boolean");
        public static readonly LuaType LightUserData = new LuaType(2, "LightUserData");
        public static readonly LuaType Number = new LuaType(3, "Number");
        public static readonly LuaType String = new LuaType(4, "String");
        public static readonly LuaType Table = new LuaType(5, "Table");
        public static readonly LuaType Function = new LuaType(6, "Function");
        public static readonly LuaType UserData = new LuaType(7, "UserData");
        public static readonly LuaType Thread = new LuaType(8, "Thread");

        readonly Int32 type;
        readonly String name;
        private LuaType(Int32 type, String name = "")
        {
            this.type = type;
            this.name = name;
        }

        public static implicit operator Int32(LuaType value) => value.type;

        public static implicit operator LuaType(Int32 value) => new LuaType(value);
    }
}
