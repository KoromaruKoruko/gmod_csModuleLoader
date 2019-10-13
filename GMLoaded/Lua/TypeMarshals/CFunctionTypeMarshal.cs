using System;
using System.Runtime.InteropServices;

namespace GMLoaded.Lua.TypeMarshals
{
    public class CFunctionTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => GLua.IsType(stackPos, LuaType.Function) ? Marshal.GetDelegateForFunctionPointer<CFunc>(GLua.LuaBase.GetCFunction(stackPos)) : null;

        public void Push(GLua GLua, Object obj)
        {
            if (obj is CFunc func)
                GLua.PushCFunction(func);
        }

        private CFunctionTypeMarshal() { }
        private static CFunctionTypeMarshal Instance;
        public static CFunctionTypeMarshal Create()
        {
            Instance = Instance ?? new CFunctionTypeMarshal();
            return Instance;
        }
    }
}
