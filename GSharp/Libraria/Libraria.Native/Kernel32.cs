using System;
using System.Runtime.InteropServices;

namespace Libraria.Native
{
    [Flags]
    public enum MemProtection : UInt32
    {
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        Exec = 0x10,
        ExecRead = 0x20,
        ExecReadWrite = 0x40,
        ExecWriteCopy = 0x80,
        PageGuard = 0x100,
        NoCache = 0x200,
        WriteCombine = 0x400
    }

    public static class Kernel32
    {
        [DllImport("kernel32")]
        public static extern Boolean AllocConsole();

        [DllImport("kernel32")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetModuleHandle(String ModuleName);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr GetProcAddress(IntPtr Lib, String ProcName);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr LoadLibrary(String Name);

        [DllImport("kernel32")]
        public static extern IntPtr RtlPcToFileHeader(IntPtr PC, out IntPtr Base);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr Addr, IntPtr Size,
            AllocationType AllocType = AllocationType.COMMIT, MemoryProtection MemProtect = MemoryProtection.EXECUTE_READWRITE);

        [DllImport("kernel32", SetLastError = true)]
        public static extern Boolean VirtualFree(IntPtr Addr, IntPtr Size, AllocationType FreeType);

        public static Boolean VirtualFree(IntPtr Addr) => VirtualFree(Addr, IntPtr.Zero, AllocationType.RELEASE);

        [DllImport("kernel32", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern Boolean VirtualProtect(IntPtr Addr, UInt32 Size, MemProtection NewProtect, out MemProtection OldProtect);

        public static Boolean VirtualProtect(IntPtr Addr, Int32 Size, MemProtection NewProtect, out MemProtection OldProtect) => VirtualProtect(Addr, (UInt32)Size, NewProtect, out OldProtect);

        public static Boolean VirtualProtect(IntPtr Addr, UInt32 Size, MemProtection NewProtect) => VirtualProtect(Addr, Size, NewProtect, out MemProtection Old);

        public static Boolean VirtualProtect(IntPtr Addr, Int32 Size, MemProtection NewProtect) => VirtualProtect(Addr, (UInt32)Size, NewProtect);
    }
}
