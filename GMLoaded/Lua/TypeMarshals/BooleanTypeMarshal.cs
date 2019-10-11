using System;
namespace GMLoaded.Lua.TypeMarshals
{
    public class BooleanTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => GLua.IsType(stackPos, LuaType.Boolean) ? GLua.LuaBase.GetBool(stackPos) : (Object)null;

        public void Push(GLua GLua, Object obj)
        {
            if (obj is Boolean val)
            {
                GLua.LuaBase.PushBool(val);
            }
        }
    }
}
