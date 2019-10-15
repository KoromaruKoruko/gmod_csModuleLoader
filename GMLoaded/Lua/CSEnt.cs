using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class CSEnt : Entity
    {
        public CSEnt(SmartLuaReferance Ref) : base(Ref)
        {
        }

        public CSEnt(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        {
        }
        public void Remove()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Remove");
            this.LuaHandle.LuaBase.Push(-2);
            this.LuaHandle.Call(1, 0);

            if (B)
                this.LuaHandle.UnLock();

            this.Dispose();
        }
    }
}
