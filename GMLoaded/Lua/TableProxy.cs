using System;
namespace GMLoaded.Lua
{
    public class TableProxy : IDisposable
    {
        public readonly Int32 TableRefrance;
        public readonly GLua LuaHandle;
        public TableProxy(GLua LuaHandle, Int32 IStackPos)
        {
            this.LuaHandle = LuaHandle;
            this.LuaHandle.Lock();

            this.LuaHandle.Push(IStackPos);
            this.LuaHandle.ReferenceCreate();
            this.LuaHandle.Pop();

            this.LuaHandle.UnLock();
        }

        public void Push()
        {
            this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.TableRefrance);
            this.LuaHandle.UnLock();
        }

        public void Dispose() => this.LuaHandle.ReferenceFree(this.TableRefrance);
    }
}
