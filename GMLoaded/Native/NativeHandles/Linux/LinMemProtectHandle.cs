using System;
namespace GMLoaded.Native.NativeHandles.Linux
{
    // TODO: Make a function to read memorymappings from "/proc/self/maps", because this is is not carefull about memory protection
    //          exmp: https://github.com/18446744073709551615/reDroid/blob/master/jni/reDroid/re_mprot.c (Will want a version outside of C)

    /// <summary>
    /// <para>A handle to safely temporarily change memory protection.</para>
    /// <para>The protection is reset when the handle is disposed.</para>
    /// </summary>
    public class LinMemProtectHandle : Natives.GenericMemory.IMemProtectHandle
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

        Natives.GenericMemory.GMemProtectionFlags Natives.GenericMemory.IMemProtectHandle.OldProtection => Natives.GenericMemory.GMemProtectionFlags.NONE;
        //{
        //    get
        //    {
        //        GMemProtectionFlags Flags = (GMemProtectionFlags)0;
        //        if (this.OldProtection.HasFlag(Natives.Libc.Protflags.PROT_WRITE))
        //            Flags |= GMemProtectionFlags.Write;
        //        if (this.OldProtection.HasFlag(Natives.Libc.Protflags.PROT_READ))
        //            Flags |= GMemProtectionFlags.Read;
        //        if (this.OldProtection.HasFlag(Natives.Libc.Protflags.PROT_EXEC))
        //            Flags |= GMemProtectionFlags.Execute;
        //        return Flags;
        //    }
        //}

        /// <summary>
        /// Creates a new <see cref="Natives.GenericMemory.Protect"/> instance with the given parameters.
        /// </summary>
        /// <param name="address">The address of the first byte that had its protection changed.</param>
        /// <param name="size">The size of the memory range with changed protection.</param>
        /// <param name="oldProtection">The previous protection setting.</param>
        public LinMemProtectHandle(IntPtr address, Int32 size)
        {
            this.Address = address;
            this.Size = size;
        }

        /// <summary>
        /// Resets the memory protection to its previous value.
        /// </summary>
        public void Dispose() { } // => Natives.Libc.MProtect(this.Address, this.Size, this.OldProtection);
    }
}
