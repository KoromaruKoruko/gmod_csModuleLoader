using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class Table : IDisposable
    {
        public Object this[Int32 Index]
        {
            get => this.GetField(Index.ToString());
            set => this.SetField(Index.ToString(), value);
        }
        public Object this[Int64 Index]
        {
            get => this.GetField(Index.ToString());
            set => this.SetField(Index.ToString(), value);
        }
        public Object this[Double Index]
        {
            get => this.GetField(Index.ToString());
            set => this.SetField(Index.ToString(), value);
        }
        public Object this[String Key]
        {
            get => this.GetField(Key);
            set => this.SetField(Key, value);
        }

        public readonly SmartLuaReferance SmartRef;

        public Table(GLua LuaHandle, Int32 IStackPos)
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
        public Table(SmartLuaReferance Ref)
        {
            this.SmartRef.Create();
            this.SmartRef = Ref;
        }
        public Table(GLua LuaHandle)
        {
            Boolean B = LuaHandle.Lock();

            LuaHandle.CreateTable();
            this.SmartRef = new SmartLuaReferance(LuaHandle, LuaHandle.ReferenceCreate());
            LuaHandle.Pop();

            if (B)
                LuaHandle.UnLock();
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
        public T GetField<T>(String Field)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, Field);
            T Ret = this.LuaHandle.Get<T>();
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
        public void SetField<T>(String Field, T Obj)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.Push<T>(Obj);
            this.LuaHandle.SetField(-2, Field);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
    }
}
