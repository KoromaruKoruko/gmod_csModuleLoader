using System;

namespace GMLoaded.Lua.TypeMarshals
{
    public class StringTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => GLua.IsType(stackPos, LuaType.String) ? GLua.LuaBase.GetString(stackPos, IntPtr.Zero) : null;

        public void Push(GLua GLua, Object obj)
        {
            if (obj is String)
            {
                String str = obj as String;
                GLua.LuaBase.PushString(str, Convert.ToUInt32(str.Length));
            }
        }

        private StringTypeMarshal() { }
        private static StringTypeMarshal Instance;
        public static StringTypeMarshal Create()
        {
            Instance = Instance ?? new StringTypeMarshal();
            return Instance;
        }
    }
}
