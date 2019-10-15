using System;
using GMLoaded.Lua.TypeMarshals;

namespace GMLoaded.Lua
{
    public class IRestore : Table
    {
        public IRestore(GLua GLua, Int32 IStackPos) : base(GLua, IStackPos)
        {
        }

        public void EndBlock()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "EndBlock");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
        public String StartBlock()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "StartBlock");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            String Str = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            if (B)
                this.LuaHandle.UnLock();
            return Str;
        }

        public Angle ReadAngle()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "ReadAngle");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Angle Ret = AngleTypeMarshal.Create().GetT(this.LuaHandle);
            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Boolean ReadBool()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "ReadBool");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Boolean Ret = this.LuaHandle.LuaBase.GetBool();
            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Entity ReadEntity()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "ReadEntity");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Entity Ent = GenericTableTypeMarshal.Create<Entity>().GetT(this.LuaHandle, -1);
            if (B)
                this.LuaHandle.UnLock();
            return Ent;
        }
        public Double ReadFloat()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "ReadFloat");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Double Ret = this.LuaHandle.LuaBase.GetNumber();
            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Double ReadInt()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "ReadInt");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Double Ret = this.LuaHandle.LuaBase.GetNumber();
            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public String ReadString()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "ReadString");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            String Str = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            if (B)
                this.LuaHandle.UnLock();
            return Str;
        }
        public Vector ReadVector()
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "ReadVector");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.Call(1, 1);
            Vector Ret = VectorTypeMarshal.Create().GetT(this.LuaHandle);
            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
    }
}
