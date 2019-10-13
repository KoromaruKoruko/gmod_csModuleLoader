using System;

namespace GMLoaded.Lua.TypeMarshals
{
    public class NumberTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => GLua.IsType(stackPos, LuaType.Number) ? GLua.LuaBase.GetNumber(stackPos) : (Object)null;
        public void Push(GLua GLua, Object obj) => GLua.LuaBase.PushNumber(Convert.ToDouble(obj));

        private NumberTypeMarshal() { }
        private static NumberTypeMarshal Instance;
        public static NumberTypeMarshal Create()
        {
            Instance = Instance ?? new NumberTypeMarshal();
            return Instance;
        }
    }
}
