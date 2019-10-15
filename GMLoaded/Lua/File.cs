using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class File : Table
    {
        public File(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        {
        }
        public File(SmartLuaReferance Ref) : base(Ref)
        {
        }

        public void Close()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Close");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
        public void Flush()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Flush");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
        public String Read(Int32 nChar)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Read");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(nChar);
            this.LuaHandle.Call(2, 1);
            String Ret = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Boolean ReadBool()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadBool");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Boolean Ret = this.LuaHandle.LuaBase.GetBool();
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Byte ReadByte()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadByte");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Byte Ret = Convert.ToByte(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Double ReadDouble()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadDouble");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Double Ret = this.LuaHandle.LuaBase.GetNumber();
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Single ReadFloat()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadFloat");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Single Ret = Convert.ToSingle(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public String ReadLine()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadLine");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            String Ret = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Int64 ReadLong()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadLong");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Int64 Ret = Convert.ToInt64(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Int16 ReadShort()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadShort");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Int16 Ret = Convert.ToInt16(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public UInt64 ReadULong()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadULong");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            UInt64 Ret = Convert.ToUInt64(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public UInt16 ReadUShort()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "ReadUShort");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            UInt16 Ret = Convert.ToUInt16(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public void Seek(Int64 Pos)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Seek");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Pos);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public Int64 Size()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Size");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Int64 Ret = Convert.ToInt64(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Int64 Skip(Int64 Amount)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Skip");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Amount);
            this.LuaHandle.Call(2, 1);
            Int64 Ret = Convert.ToInt64(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public Int64 Tell()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Tell");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Int64 Ret = Convert.ToInt64(this.LuaHandle.LuaBase.GetNumber());
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
            return Ret;
        }
        public void Write(String String)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "Write");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushString(String, Convert.ToUInt32(String.Length));
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteBool(Boolean Bool)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteBool");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushBool(Bool);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteByte(Byte Byte)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteByte");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Byte);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteDouble(Double Double)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteDouble");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Double);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteFloat(Single Float)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteFloat");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Float);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteLong(Int64 Long)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteLong");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Long);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteShort(Int16 Short)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteShort");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Short);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteULong(UInt64 ULong)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteULong");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(ULong);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
        public void WriteUShort(UInt16 UShort)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "WriteUShort");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(UShort);
            this.LuaHandle.Call(2, 0);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();
        }
    }
}
