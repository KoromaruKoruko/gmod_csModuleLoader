using System;
using System.Runtime.InteropServices;

namespace GMLoaded.Lua.TypeMarshals
{
    public class AngleTypeMarshal : ILuaTypeMarshal
    {
        public Angle GetT(GLua GLua, Int32 stackPos = -1) => Marshal.PtrToStructure<Angle>(GLua.LuaBase.GetVector(stackPos));

        public Object Get(GLua GLua, Int32 stackPos = -1) => Marshal.PtrToStructure<Angle>(GLua.LuaBase.GetVector(stackPos));
        public void Push(GLua GLua, Object obj)
        {
            IntPtr Ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Angle>());
            Marshal.StructureToPtr<Angle>((Angle)obj, Ptr, true);
            GLua.LuaBase.PushAngle(Ptr);

            Marshal.FreeHGlobal(Ptr);
        }

        private AngleTypeMarshal() { }
        private static AngleTypeMarshal Instance;
        public static AngleTypeMarshal Create()
        {
            Instance = Instance ?? new AngleTypeMarshal();
            return Instance;
        }
    }
}
