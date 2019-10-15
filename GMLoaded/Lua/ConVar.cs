using System;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public class ConVar : Table
    {
        public ConVar(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        {
        }
        public ConVar(SmartLuaReferance Ref) : base(Ref)
        {
        }

        public Boolean GetBool()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "GetBool");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Boolean Ret = this.LuaHandle.LuaBase.GetBool();
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }
        public String GetDefult()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "GetDefult");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            String Ret = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }
        public Double GetFloat()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "GetFloat");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Double Ret = this.LuaHandle.LuaBase.GetNumber();
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }
        public String GetHelpText()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "GetHelpText");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            String Ret = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }
        public Double GetInt()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "GetInt");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            Double Ret = this.LuaHandle.LuaBase.GetNumber();
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }
        public String GetName()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "GetName");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            String Ret = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }
        public String GetString()
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "GetString");
            this.LuaHandle.Push(-2);
            this.LuaHandle.Call(1, 1);
            String Ret = this.LuaHandle.LuaBase.GetString(-1, IntPtr.Zero);
            this.LuaHandle.Pop();

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }
        public void SetBool(Boolean Bool)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "SetBool");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushBool(Bool);
            this.LuaHandle.Call(2, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
        public void SetFloat(Double Double)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "SetFloat");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Double);
            this.LuaHandle.Call(2, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
        public void SetInt(Double Int)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "SetInt");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushNumber(Int);
            this.LuaHandle.Call(2, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
        public void SetString(String String)
        {
            Boolean B = this.LuaHandle.Lock();

            this.Push();
            this.LuaHandle.GetField(-1, "SetString");
            this.LuaHandle.Push(-2);
            this.LuaHandle.LuaBase.PushString(String, Convert.ToUInt32(String.Length));
            this.LuaHandle.Call(2, 0);

            if (B)
                this.LuaHandle.UnLock();
        }
    }
}
