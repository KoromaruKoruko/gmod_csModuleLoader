using System;

namespace GMLoaded.Lua.LibraryWrappers
{
    public class Hooks : IDisposable
    {
        private readonly Int32 Hook_Add;
        private readonly Int32 Hook_Call;
        private readonly Int32 Hook_GetTable;
        private readonly Int32 Hook_Remove;
        private readonly Int32 Hook_Run;

        public readonly GLua LuaHandle;

        public Hooks(GLua GLua)
        {
            this.LuaHandle = GLua;
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.GetGlobal("hook");
            {
                this.LuaHandle.GetField(-1, "Add");
                this.Hook_Add = this.LuaHandle.ReferenceCreate();
                this.LuaHandle.Pop();

                this.LuaHandle.GetField(-1, "Call");
                this.Hook_Call = this.LuaHandle.ReferenceCreate();
                this.LuaHandle.Pop();

                this.LuaHandle.GetField(-1, "GetTable");
                this.Hook_GetTable = this.LuaHandle.ReferenceCreate();
                this.LuaHandle.Pop();

                this.LuaHandle.GetField(-1, "Remove");
                this.Hook_Remove = this.LuaHandle.ReferenceCreate();
                this.LuaHandle.Pop();

                this.LuaHandle.GetField(-1, "Run");
                this.Hook_Run = this.LuaHandle.ReferenceCreate();
            }
            this.LuaHandle.Pop(2);

            if (B)
                this.LuaHandle.UnLock();

            this.LuaHandle.OnClose += this.LuaHandle_OnClose;
        }

        private void LuaHandle_OnClose(GLua GLua) => this.Dispose();

        public void Call(String Hook, Int32 GamemodeTablePtr, params Object[] Args)
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Hook_Call);
            this.LuaHandle.LuaBase.PushString(Hook);
            this.LuaHandle.LuaBase.Push(GamemodeTablePtr);

            foreach (Object A in Args)
                this.LuaHandle.Push(A, A.GetType());

            this.LuaHandle.Call(Args.Length + 2, 0);

            if (B)
                this.LuaHandle.UnLock();
        }

        public Object[] Call(Int32 ReturnCount, String Hook, Int32 GamemodeTablePtr, params Object[] Args)
        {
            if (ReturnCount > 6 || ReturnCount < 0)
                throw new ArgumentOutOfRangeException("ReturnCount", "Return Count is between 0-6");

            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Hook_Call);
            this.LuaHandle.LuaBase.PushString(Hook);
            this.LuaHandle.LuaBase.Push(GamemodeTablePtr);

            foreach (Object A in Args)
                this.LuaHandle.Push(A, A.GetType());

            this.LuaHandle.Call(Args.Length + 2, ReturnCount);

            Object[] Ret = new Object[ReturnCount];

            for (Int32 x = 0; x < ReturnCount; x++)
                Ret[x] = this.LuaHandle.PopGet();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }

        public Table GetTable()
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Hook_GetTable);
            this.LuaHandle.Call(0, 1);
            Table Table = new Table(this.LuaHandle, -1);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Table;
        }

        public void Remove(String Hook, Object Identifier)
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Hook_Remove);
            this.LuaHandle.LuaBase.PushString(Hook, (UInt32)Hook.Length);
            this.LuaHandle.Push(Identifier, Identifier.GetType());
            this.LuaHandle.Call(2, 0);

            if (B)
                this.LuaHandle.UnLock();
        }

        public void Run(String Hook, params Object[] Args)
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Hook_Run);
            this.LuaHandle.LuaBase.PushString(Hook);

            foreach (Object A in Args)
                this.LuaHandle.Push(A, A.GetType());

            this.LuaHandle.Call(Args.Length + 1, 0);

            if (B)
                this.LuaHandle.UnLock();
        }

        public Object[] Run(Int32 ReturnCount, String Hook, params Object[] Args)
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Hook_Run);
            this.LuaHandle.LuaBase.PushString(Hook);

            foreach (Object A in Args)
                this.LuaHandle.Push(A, A.GetType());

            this.LuaHandle.Call(Args.Length + 1, ReturnCount);

            Object[] Ret = new Object[ReturnCount];

            for (Int32 x = 0; x < ReturnCount; x++)
                Ret[x] = this.LuaHandle.PopGet();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }

        public void Dispose()
        {
            this.LuaHandle.ReferenceFree(this.Hook_Add);
            this.LuaHandle.ReferenceFree(this.Hook_Call);
            this.LuaHandle.ReferenceFree(this.Hook_GetTable);
            this.LuaHandle.ReferenceFree(this.Hook_Remove);
            this.LuaHandle.ReferenceFree(this.Hook_Run);
            this.LuaHandle.OnClose -= this.LuaHandle_OnClose;
        }
    }
}
