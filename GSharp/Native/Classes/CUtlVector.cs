using System;
using System.Runtime.InteropServices;

namespace GSharp.Native.Classes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct CUtlVector<T> where T : struct
    {
        public IntPtr CUtlVecPtr;

        Boolean NotNull(Int32 Idx) => this.Element(Idx) != IntPtr.Zero;

        public Int32 GetCount()
        {
            Int32 i;
            for (i = 0; this.NotNull(i); i++) ;
            return i;
        }

        public IntPtr Element(Int32 Idx)
        {
            IntPtr Mem = Marshal.ReadIntPtr(this.CUtlVecPtr);
            if (Mem == IntPtr.Zero)
                return IntPtr.Zero;

            Int32 TSize = Marshal.SizeOf(typeof(T));
            return Marshal.ReadIntPtr(Mem, TSize * Idx);
        }

        public IntPtr this[Int32 Idx] => this.Element(Idx);

        public static implicit operator CUtlVector<T>(IntPtr Ptr)
        {
            CUtlVector<T> Vec = new CUtlVector<T>
            {
                CUtlVecPtr = Ptr
            };
            return Vec;
        }
    }
}