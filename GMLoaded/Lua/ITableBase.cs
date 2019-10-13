using System;
namespace GMLoaded.Lua
{
    public class ITableBase : IDisposable
    {
        public readonly Int32 Refrance;
        public readonly GLua LuaHandle;
        public ITableBase(GLua LuaHandle, Int32 IStackPos)
        {
            this.LuaHandle = LuaHandle;
            this.LuaHandle.Lock();

            this.LuaHandle.Push(IStackPos);
            this.LuaHandle.ReferenceCreate();
            this.LuaHandle.Pop();

            this.LuaHandle.UnLock();
        }
        public ITableBase(Int32 Refrance, GLua LuaHandle)
        {
            this.Refrance = Refrance;
            this.LuaHandle = LuaHandle;
        }

        public void Push() => this.LuaHandle.ReferencePush(this.Refrance);

        public void Dispose() => this.LuaHandle.ReferenceFree(this.Refrance);
    }
}
