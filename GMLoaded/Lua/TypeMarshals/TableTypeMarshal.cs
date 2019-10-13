using System;

namespace GMLoaded.Lua.TypeMarshals
{
    public class TableTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => new ITableBase(GLua, stackPos);
        public void Push(GLua GLua, Object obj)
        {
            if (!(obj is ITableBase Proxy))
                throw new InvalidCastException();
            if (GLua != Proxy.LuaHandle)
                throw new InvalidOperationException("You cant transfer tables between LuaStates");
            GLua.ReferencePush(Proxy.Refrance);
        }

        private TableTypeMarshal() { }
        private static TableTypeMarshal Instance;
        public static TableTypeMarshal Create()
        {
            Instance = Instance ?? new TableTypeMarshal();
            return Instance;
        }
    }
}
