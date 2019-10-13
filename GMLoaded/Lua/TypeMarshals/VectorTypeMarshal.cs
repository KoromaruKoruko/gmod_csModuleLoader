using System;
using System.Runtime.InteropServices;

namespace GMLoaded.Lua.TypeMarshals
{
    public class VectorTypeMarshal : ILuaTypeMarshal
    {
        public Vector GetT(GLua GLua, Int32 stackPos = -1) => Marshal.PtrToStructure<Vector>(GLua.LuaBase.GetVector(stackPos));

        public Object Get(GLua GLua, Int32 stackPos = -1) => Marshal.PtrToStructure<Vector>(GLua.LuaBase.GetVector(stackPos));
        public void Push(GLua GLua, Object obj)
        {
            IntPtr Ptr = Marshal.AllocHGlobal(Marshal.SizeOf<Vector>());
            Marshal.StructureToPtr<Vector>((Vector)obj, Ptr, false);
            GLua.LuaBase.PushVector(Ptr);
            Marshal.FreeHGlobal(Ptr);
        }

        private VectorTypeMarshal() { }
        private static VectorTypeMarshal Instance;
        public static VectorTypeMarshal Create()
        {
            Instance = Instance ?? new VectorTypeMarshal();
            return Instance;
        }
    }
}
