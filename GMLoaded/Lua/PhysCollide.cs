using System;
using GMLoaded.Lua.TypeMarshals;
using GMLoaded.Lua.Smart;

namespace GMLoaded.Lua
{
    public struct PhysCollideTraceBoxReturn
    {
        public Vector HitPos, HitNormal;
        public Double TraceFraction;
    }

    public class PhysCollide : Table
    {
        public PhysCollide(GLua LuaHandle, Int32 IStackPos) : base(LuaHandle, IStackPos)
        { }
        public PhysCollide(SmartLuaReferance SmartRef) : base(SmartRef)
        { }

        public PhysCollideTraceBoxReturn TraceBox(Vector Origin, Angle Angles, Vector RayStart, Vector RayEnd, Vector RayMins, Vector RayMaxs)
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "TraceBox");
            this.LuaHandle.Push(-2);

            // Going to keep this at the marshal level, in case of future gmod updates or if someone wants to log marshals
            VectorTypeMarshal Marshal = VectorTypeMarshal.Create();
            AngleTypeMarshal AMarshal = AngleTypeMarshal.Create();

            Marshal.Push(this.LuaHandle, Origin);
            AMarshal.Push(this.LuaHandle, Angles);
            Marshal.Push(this.LuaHandle, RayStart);
            Marshal.Push(this.LuaHandle, RayEnd);
            Marshal.Push(this.LuaHandle, RayMins);
            Marshal.Push(this.LuaHandle, RayMaxs);

            this.LuaHandle.Call(7, 3);
            PhysCollideTraceBoxReturn Ret = new PhysCollideTraceBoxReturn
            {
                HitPos = Marshal.GetT(this.LuaHandle, -3),
                HitNormal = Marshal.GetT(this.LuaHandle, -2),
                TraceFraction = this.LuaHandle.LuaBase.GetNumber(-1),
            };
            this.LuaHandle.Pop(4);

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }

        public Boolean IsValid()
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Referance);

            this.LuaHandle.GetField(-1, "IsValid");
            this.LuaHandle.Push(-1);
            this.LuaHandle.Call(1, 1);

            Boolean Ret = this.LuaHandle.LuaBase.GetBool();

            this.LuaHandle.Pop(2);

            if (B)
                this.LuaHandle.UnLock();

            return Ret;
        }

        public void Destroy()
        {
            Boolean B = this.LuaHandle.Lock();

            this.LuaHandle.ReferencePush(this.Referance);
            this.LuaHandle.GetField(-1, "Destroy");
            this.LuaHandle.LuaBase.Insert(-2);
            this.LuaHandle.Call(1, 0);

            if (B)
                this.LuaHandle.UnLock();

            this.Dispose();
        }
    }
}
