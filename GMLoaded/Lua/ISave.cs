using System;
using GMLoaded.Lua.TypeMarshals;

namespace GMLoaded.Lua
{
    public class ISave : Table
    {
        public ISave(GLua Glua, Int32 IStackPos) : base(Glua, IStackPos)
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
        public void StartBlock(String Name)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "StartBlock");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushString(Name, (UInt32)Name.Length);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }

        public void WriteAngle(Angle Angle)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "WriteAngle");
            this.LuaHandle.Insert(-2);
            AngleTypeMarshal.Create().Push(this.LuaHandle, Angle);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteBool(Boolean Bool)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "WriteBool");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushBool(Bool);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteEntity(Entity Ent)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "WriteEntity");
            this.LuaHandle.Insert(-2);
            Ent.Push();
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteFloat(Double Float)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "WriteFloat");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushNumber(Float);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteInt(Double Int)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "WriteInt");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushNumber(Int);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteString(String String)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "WriteString");
            this.LuaHandle.Insert(-2);
            this.LuaHandle.LuaBase.PushString(String, (UInt32)String.Length);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteVector(Vector Vector)
        {
            Boolean B = this.LuaHandle.Lock();
            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "WriteVector");
            this.LuaHandle.Insert(-2);
            VectorTypeMarshal.Create().Push(this.LuaHandle, Vector);
            this.LuaHandle.Call(2, 0);
            if (B)
                this.LuaHandle.UnLock();
        }
    }
}
