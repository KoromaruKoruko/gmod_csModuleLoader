using System;

namespace GMLoaded.Lua.TypeMarshals
{
    public class CFunctionTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => GLua.IsType(stackPos, LuaType.Function) ? GLua.LuaBase.GetCFunction(stackPos) as Object : null;

        public void Push(GLua GLua, Object obj)
        {
            if (obj is CFunc func)
                GLua.PushCFunction(func);
        }
    }
}
