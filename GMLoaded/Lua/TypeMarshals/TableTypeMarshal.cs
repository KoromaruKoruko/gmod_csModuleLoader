using System;

namespace GMLoaded.Lua.TypeMarshals
{
    public class TableTypeMarshal : ILuaTypeMarshal
    {
        public Object Get(GLua GLua, Int32 stackPos = -1) => new Table(GLua, stackPos);
        public void Push(GLua GLua, Object obj)
        {
            if (!(obj is Table Proxy))
                throw new InvalidCastException();
            if (GLua != Proxy.LuaHandle)
                throw new InvalidOperationException("You cant transfer tables between LuaStates");
            GLua.ReferencePush(Proxy.Referance);
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
