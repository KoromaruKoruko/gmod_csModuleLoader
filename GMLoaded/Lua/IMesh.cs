using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class IMesh : Table
    {
        public IMesh(SmartLuaReferance Ref) : base(Ref)
        {
        }

        public IMesh(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        {
        }

        public void BuildFromTriangles(Table Vertexes)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "BuildFromTriangles");
            this.LuaHandle.Push(-2);
            Vertexes.Push();
            this.LuaHandle.Call(2, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
        public void Destroy()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Destroy");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
        public void Draw()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Draw");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
    }
}
