using System;
using GMLoaded.Native.NativeHandles.Windows;
using GMLoaded.Native.NativeHandles.Linux;
using System.Runtime.InteropServices;

namespace GMLoaded.Native
{
    public static partial class Natives
    {
        public static class GenericMemory
        {
            /// <summary>
            /// General Memory Protection flags, For use within the GMemProtect Handle
            /// </summary>
            [Flags]
            public enum GMemProtectionFlags
            {
                NONE = 0,
                Read = 1,
                Write = 2,
                ReadWrite = 3,
                Execute = 4,
                ExecuteRead = 5,
                ExecuteWrite = 6,
                ALL = 7,
            }

            /// <summary>
            /// Memory Protection Handle
            /// </summary>
            public interface IMemProtectHandle : IDisposable
            {
                /// <summary>
                /// The address of the first byte that had its protection changed.
                /// </summary>
                public IntPtr Address { get; }

                /// <summary>
                /// The size of the memory range with changed protection.
                /// </summary>
                public Int32 Size { get; }

                /// <summary>
                /// The previous protection setting.
                /// </summary>
                public GMemProtectionFlags OldProtection { get; }
            }

            public static IMemProtectHandle Protect(IntPtr Start, Int32 Count, GMemProtectionFlags Flags)
            {
                switch (SystemType)
                {
                    case System.Linux:
                    {
                        Libc.Protflags nFlags = Libc.Protflags.PROT_NONE;

                        if (Flags.HasFlag(GMemProtectionFlags.Execute))
                            nFlags |= Libc.Protflags.PROT_EXEC;
                        if (Flags.HasFlag(GMemProtectionFlags.Write))
                            nFlags |= Libc.Protflags.PROT_WRITE;
                        if (Flags.HasFlag(GMemProtectionFlags.Read))
                            nFlags |= Libc.Protflags.PROT_READ;
                        Int32 x = Libc.MProtect(Start, Count, nFlags);
                        if (x == 0)
                            return new LinMemProtectHandle(Start, Count);
                        else
                            throw new Exception("Failed with error code :" + x);
                    }

                    case System.Windows:
                    {
                        Kernel32.MemoryProtection nFlags = Flags switch
                        {
                            GMemProtectionFlags.Execute => Kernel32.MemoryProtection.EXECUTE,
                            GMemProtectionFlags.Read => Kernel32.MemoryProtection.READONLY,
                            GMemProtectionFlags.Write => Kernel32.MemoryProtection.WRITECOPY,
                            GMemProtectionFlags.Execute | GMemProtectionFlags.Read => Kernel32.MemoryProtection.EXECUTE_READ,
                            GMemProtectionFlags.Execute | GMemProtectionFlags.Write => Kernel32.MemoryProtection.EXECUTE_WRITECOPY,
                            GMemProtectionFlags.Read | GMemProtectionFlags.Write => Kernel32.MemoryProtection.READWRITE,
                            GMemProtectionFlags.Execute | GMemProtectionFlags.Read | GMemProtectionFlags.Write => Kernel32.MemoryProtection.EXECUTE_READWRITE,
                            _ => Kernel32.MemoryProtection.NOACCESS,
                        };
                        if (Kernel32.VirtualProtect(Start, Count, nFlags, out Kernel32.MemoryProtection OFlags))
                            return new WinMemProtectHandle(Start, Count, OFlags);
                        else
                            throw new Exception("Failed with error code :" + Marshal.GetLastWin32Error());
                    }
                }
                throw new PlatformNotSupportedException();
            }
        }
    }
}
