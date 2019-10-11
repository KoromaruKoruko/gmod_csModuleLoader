// InBuffer.cs
using System;
using System.IO;

namespace SevenZip.Buffer
{
    public class InBuffer
    {
        private readonly Byte[] m_Buffer;
        private readonly UInt32 m_BufferSize;
        private UInt32 m_Limit;
        private UInt32 m_Pos;
        private UInt64 m_ProcessedSize;
        private Stream m_Stream;
        private Boolean m_StreamWasExhausted;

        public InBuffer(UInt32 bufferSize)
        {
            this.m_Buffer = new Byte[bufferSize];
            this.m_BufferSize = bufferSize;
        }

        public UInt64 GetProcessedSize() => this.m_ProcessedSize + this.m_Pos;

        public void Init(Stream stream)
        {
            this.m_Stream = stream;
            this.m_ProcessedSize = 0;
            this.m_Limit = 0;
            this.m_Pos = 0;
            this.m_StreamWasExhausted = false;
        }

        public Boolean ReadBlock()
        {
            if (this.m_StreamWasExhausted)
                return false;
            this.m_ProcessedSize += this.m_Pos;
            Int32 aNumProcessedBytes = this.m_Stream.Read(this.m_Buffer, 0, (Int32)this.m_BufferSize);
            this.m_Pos = 0;
            this.m_Limit = (UInt32)aNumProcessedBytes;
            this.m_StreamWasExhausted = aNumProcessedBytes == 0;
            return !this.m_StreamWasExhausted;
        }

#pragma warning disable IDE0060 // Remove unused parameter, public domain, Ignoring!

        public Boolean ReadByte(Byte b) // check it
#pragma warning restore IDE0060
        {
            if (this.m_Pos >= this.m_Limit)
                if (!this.ReadBlock())
                    return false;
            b = this.m_Buffer[this.m_Pos++];
            return true;
        }

        public System.Byte ReadByte()
        {
            // return (byte)m_Stream.ReadByte();
            if (this.m_Pos >= this.m_Limit)
                if (!this.ReadBlock())
                    return 0xFF;
            return this.m_Buffer[this.m_Pos++];
        }

        public void ReleaseStream() =>
                            // m_Stream.Close();
                            this.m_Stream = null;
    }
}
