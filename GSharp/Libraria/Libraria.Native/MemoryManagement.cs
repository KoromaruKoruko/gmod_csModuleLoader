using System;

namespace Libraria.Native
{
    /// <summary>
    /// Provides memory management convenience functions.
    /// </summary>
    public static class MemoryManagement
    {
        /// <summary>
        /// <para>A handle to safely temporarily change memory protection.</para>
        /// <para>The protection is reset when the handle is disposed.</para>
        /// </summary>
        public class VirtualProtectHandle : IDisposable
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
            public MemProtection OldProtection
            {
                get;
                private set;
            }

            /// <summary>
            /// Creates a new <see cref="VirtualProtectHandle"/> instance with the given parameters.
            /// </summary>
            /// <param name="address">The address of the first byte that had its protection changed.</param>
            /// <param name="size">The size of the memory range with changed protection.</param>
            /// <param name="oldProtection">The previous protection setting.</param>
            public VirtualProtectHandle(IntPtr address, Int32 size, MemProtection oldProtection)
            {
                this.Address = address;
                this.Size = size;
                this.OldProtection = oldProtection;
            }

            /// <summary>
            /// Resets the memory protection to its previous value.
            /// </summary>
            public void Dispose() => Kernel32.VirtualProtect(this.Address, this.Size, this.OldProtection);
        }

        public static VirtualProtectHandle Protect(IntPtr address, Int32 size, MemProtection newProtection)
        {
            Kernel32.VirtualProtect(address, size, newProtection, out MemProtection oldProtection);
            return new VirtualProtectHandle(address, size, oldProtection);
        }
    }
}