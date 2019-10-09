using System;
using System.Runtime.InteropServices;

namespace Libraria.Native
{
    public static class MachineCode
    {
        public static IntPtr AllocMem(IntPtr BaseAddr, Int32 Size) => Kernel32.VirtualAlloc(BaseAddr, (IntPtr)Size, AllocationType.COMMIT, MemoryProtection.EXECUTE_READWRITE);

        public static void Link(IntPtr Addr, Delegate Func)
        {
            IntPtr FuncPtr = Marshal.GetFunctionPointerForDelegate(Func);

            Byte[] LinkCode = Asmblr.CreateByteArray(
                (Byte)0x50, (Byte)0x50,                                         // push RAX; push RAX
                (Byte)0x48, (Byte)0xB8, FuncPtr.ToInt64(),                      // movabs RAX, FuncPtr
                (Byte)0x48, (Byte)0x89, (Byte)0x44, (Byte)0x24, (Byte)0x08,     // mov QWORD PTR [RSP + 0x8], RAX
                (Byte)0x58, (Byte)0xC3                                          // pop RAX; ret
                );

            Marshal.Copy(LinkCode, 0, Addr, LinkCode.Length);
        }

        public static void Link(Int32 Addr, Delegate Func) => Link((IntPtr)Addr, Func);

        public static void Link(Int32 Addr, Action Func) => Link((IntPtr)Addr, Func);

        public static T ToDelegate<T>(Byte[] Code, out IntPtr FuncPtr) where T : class
        {
            FuncPtr = ToFunction(Code);
            return Marshal.GetDelegateForFunctionPointer<T>(FuncPtr);
        }

        public static T ToDelegate<T>(Byte[] Code) where T : class => ToDelegate<T>(Code, out IntPtr FuncPtr);

        public static IntPtr ToFunction(Byte[] Code)
        {
            IntPtr Ptr = AllocMem(IntPtr.Zero, Code.Length);
            Marshal.Copy(Code, 0, Ptr, Code.Length);
            return Ptr;
        }
    }
}
