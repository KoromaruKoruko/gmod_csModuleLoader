using System;

namespace GMLoaded.Lua.TypeMarshals
{
    public class NumberTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1)
        {
            if (GLua.IsType(stackPos, LuaType.Number))
            {
                return GLua.LuaBase.GetNumber(stackPos);
            }
            return null;
        }

        public void Push(GLua GLua, Object obj) => GLua.LuaBase.PushNumber(Convert.ToDouble(obj));
    }
}
