using System;

namespace GSharp.GLuaNET.TypeMarshals
{
    public class StringTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1)
        {
            if (GLua.IsType(stackPos, LuaType.String))
            {
                return GLua.LuaBase.GetString(stackPos, IntPtr.Zero);
            }
            return null;
        }

        public void Push(GLua GLua, Object obj)
        {
            if (obj is String)
            {
                String str = obj as String;
                GLua.LuaBase.PushString(str, Convert.ToUInt32(str.Length));
            }
        }
    }
}