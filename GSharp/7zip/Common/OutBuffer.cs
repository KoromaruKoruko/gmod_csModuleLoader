// OutBuffer.cs

namespace SevenZip.Buffer
{
    public class OutBuffer
    {
        readonly System.Byte[] m_Buffer;
        System.UInt32 m_Pos;
        readonly System.UInt32 m_BufferSize;
        System.IO.Stream m_Stream;
        System.UInt64 m_ProcessedSize;

        public OutBuffer(System.UInt32 bufferSize)
        {
            this.m_Buffer = new System.Byte[bufferSize];
            this.m_BufferSize = bufferSize;
        }

        public void SetStream(System.IO.Stream stream) => this.m_Stream = stream;
        public void FlushStream() => this.m_Stream.Flush();
        public void CloseStream() => this.m_Stream.Close();
        public void ReleaseStream() => this.m_Stream = null;

        public void Init()
        {
            this.m_ProcessedSize = 0;
            this.m_Pos = 0;
        }

        public void WriteByte(System.Byte b)
        {
            this.m_Buffer[this.m_Pos++] = b;
            if (this.m_Pos >= this.m_BufferSize)
                this.FlushData();
        }

        public void FlushData()
        {
            if (this.m_Pos == 0)
                return;
            this.m_Stream.Write(this.m_Buffer, 0, (System.Int32)this.m_Pos);
            this.m_Pos = 0;
        }

        public System.UInt64 GetProcessedSize() => this.m_ProcessedSize + this.m_Pos;
    }
}