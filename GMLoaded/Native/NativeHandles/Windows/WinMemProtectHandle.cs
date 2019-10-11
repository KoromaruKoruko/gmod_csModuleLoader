using System;

namespace GMLoaded.Native.NativeHandles.Windows
{
    /// <summary>
    /// <para>A handle to safely temporarily change memory protection.</para>
    /// <para>The protection is reset when the handle is disposed.</para>
    /// </summary>
    public class WinMemProtectHandle : Natives.GenericMemory.IMemProtectHandle
    {
        /// <summary>
        /// The address of the first byte that had its protection changed.
        /// </summary>
        public IntPtr Address
        {
            get;
            private set;
        }

        /// <summary>
        /// The size of the memory range with changed protection.
        /// </summary>
        public Int32 Size
        {
            get;
            private set;
        }

        /// <summary>
        /// The previous protection setting.
        /// </summary>
        public Natives.Kernel32.MemoryProtection OldProtection
        {
            get;
            private set;
        }
        Natives.GenericMemory.GMemProtectionFlags Natives.GenericMemory.IMemProtectHandle.OldProtection
        {
            get
            {
                if (this.OldProtection.HasFlag(Natives.Kernel32.MemoryProtection.EXECUTE_READWRITE))
                    return Natives.GenericMemory.GMemProtectionFlags.ALL;
                if (this.OldProtection.HasFlag(Natives.Kernel32.MemoryProtection.READWRITE))
                    return Natives.GenericMemory.GMemProtectionFlags.ReadWrite;
                if (this.OldProtection.HasFlag(Natives.Kernel32.MemoryProtection.EXECUTE_WRITECOPY))
                    return Natives.GenericMemory.GMemProtectionFlags.ExecuteWrite;
                if (this.OldProtection.HasFlag(Natives.Kernel32.MemoryProtection.READONLY))
                    return Natives.GenericMemory.GMemProtectionFlags.Read;
                if (this.OldProtection.HasFlag(Natives.Kernel32.MemoryProtection.EXECUTE_READ))
                    return Natives.GenericMemory.GMemProtectionFlags.ExecuteRead;
                if (this.OldProtection.HasFlag(Natives.Kernel32.MemoryProtection.EXECUTE))
                    return Natives.GenericMemory.GMemProtectionFlags.Execute;
                return Natives.GenericMemory.GMemProtectionFlags.NONE;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Natives.GenericMemory.Protect"/> instance with the given parameters.
        /// </summary>
        /// <param name="address">The address of the first byte that had its protection changed.</param>
        /// <param name="size">The size of the memory range with changed protection.</param>
        /// <param name="oldProtection">The previous protection setting.</param>
        public WinMemProtectHandle(IntPtr address, Int32 size, Natives.Kernel32.MemoryProtection oldProtection)
        {
            this.Address = address;
            this.Size = size;
            this.OldProtection = oldProtection;
        }

        /// <summary>
        /// Resets the memory protection to its previous value.
        /// </summary>
        public void Dispose() => Natives.Kernel32.VirtualProtect(this.Address, this.Size, this.OldProtection);
    }
}
