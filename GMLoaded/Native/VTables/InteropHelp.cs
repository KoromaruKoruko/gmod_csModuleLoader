using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GMLoaded.Native.VTables
{
    public class InteropHelp
    {
        private static readonly GCHandle NullHandle = GCHandle.Alloc(new Byte[0], GCHandleType.Pinned);

        /// <summary>
        /// Decodes IntPtr as if it were a UTF-8 string
        /// </summary>
        public static String DecodeUTF8String(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            Int32 len = 0;
            while (Marshal.ReadByte(ptr, len) != 0) len++;

            if (len == 0)
                return String.Empty;

            Byte[] buffer = new Byte[len];
            Marshal.Copy(ptr, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Encodes string as an IntPtr
        /// </summary>
        public static IntPtr EncodeUTF8String(String str, out GCHandle handle)
        {
            if (str == null)
            {
                handle = NullHandle;
                return IntPtr.Zero;
            }

            Int32 length = Encoding.UTF8.GetByteCount(str);
            Byte[] buffer = new Byte[length + 1];

            Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 0);

            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }

        public static void FreeString(ref GCHandle handle)
        {
            if (handle == NullHandle)
                return;

            handle.Free();
        }

        public class BitVector64
        {
            public BitVector64()
            {
            }

            public BitVector64(UInt64 value) => this.Data = value;

            public UInt64 Data { get; set; }

            public UInt64 this[UInt32 bitoffset, UInt64 valuemask]
            {
                get => (this.Data >> (UInt16)bitoffset) & valuemask;
                set => this.Data = (this.Data & ~(valuemask << (UInt16)bitoffset)) | ((value & valuemask) << (UInt16)bitoffset);
            }
        }
    }
}
