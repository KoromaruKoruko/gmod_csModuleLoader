using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class ITableBase : IDisposable
    {
        public readonly SmartLuaReferance SmartRef;

        public ITableBase(GLua LuaHandle, Int32 IStackPos)
        {
            Boolean B = LuaHandle.Lock();

            if (IStackPos != -1)
            {
                LuaHandle.Push(IStackPos);
                this.SmartRef = LuaHandle.CreateSmartReferance();
                LuaHandle.Pop();
            }
            else
                this.SmartRef = LuaHandle.CreateSmartReferance();

            if (B)
                LuaHandle.UnLock();
        }

        public ITableBase(SmartLuaReferance Ref)
        {
            this.SmartRef.Create();
            this.SmartRef = Ref;
        }

        public GLua LuaHandle => this.SmartRef.Handle;
        public Int32 Referance => this.SmartRef.Referance;

        public void Dispose() => this.SmartRef.Free();

        public Object GetField(String Field)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, Field);
            Object Ret = this.LuaHandle.Get();
            this.LuaHandle.Pop(2);

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }

        public void Push() => this.LuaHandle.ReferencePush(this.Referance);

        public void SetField(String Field, Object Obj)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.Push(Obj, Obj.GetType());
            this.LuaHandle.SetField(-2, Field);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
    }
}
