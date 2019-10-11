// Common/CRC.cs
using System;

namespace SevenZip
{
    internal class CRC
    {
        private System.UInt32 _value = 0xFFFFFFFF;
        public static readonly System.UInt32[] Table;

        static CRC()
        {
            Table = new System.UInt32[256];
            const System.UInt32 kPoly = 0xEDB88320;
            for (System.UInt32 i = 0; i < 256; i++)
            {
                System.UInt32 r = i;
                for (System.Int32 j = 0; j < 8; j++)
                    if ((r & 1) != 0)
                        r = (r >> 1) ^ kPoly;
                    else
                        r >>= 1;
                Table[i] = r;
            }
        }

        private static System.UInt32 CalculateDigest(System.Byte[] data, System.UInt32 offset, System.UInt32 size)
        {
            CRC crc = new CRC();
            // crc.Init();
            crc.Update(data, offset, size);
            return crc.GetDigest();
        }

        private static System.Boolean VerifyDigest(System.UInt32 digest, System.Byte[] data, System.UInt32 offset, System.UInt32 size) => CalculateDigest(data, offset, size) == digest;

        public System.UInt32 GetDigest() => this._value ^ 0xFFFFFFFF;

        public void Init() => this._value = 0xFFFFFFFF;

        public void Update(System.Byte[] data, System.UInt32 offset, System.UInt32 size)
        {
            for (System.UInt32 i = 0; i < size; i++)
                this._value = Table[((Byte)this._value) ^ data[offset + i]] ^ (this._value >> 8);
        }

        public void UpdateByte(System.Byte b) => this._value = Table[((System.Byte)this._value) ^ b] ^ (this._value >> 8);
    }
}
