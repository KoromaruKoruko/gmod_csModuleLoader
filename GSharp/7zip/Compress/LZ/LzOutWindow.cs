// LzOutWindow.cs

namespace SevenZip.Compression.LZ
{
    public class OutWindow
    {
        System.Byte[] _buffer = null;
        System.UInt32 _pos;
        System.UInt32 _windowSize = 0;
        System.UInt32 _streamPos;
        System.IO.Stream _stream;

        public System.UInt32 TrainSize = 0;

        public void Create(System.UInt32 windowSize)
        {
            if (this._windowSize != windowSize)
            {
                // System.GC.Collect();
                this._buffer = new System.Byte[windowSize];
            }
            this._windowSize = windowSize;
            this._pos = 0;
            this._streamPos = 0;
        }

        public void Init(System.IO.Stream stream, System.Boolean solid)
        {
            this.ReleaseStream();
            this._stream = stream;
            if (!solid)
            {
                this._streamPos = 0;
                this._pos = 0;
                this.TrainSize = 0;
            }
        }

        public System.Boolean Train(System.IO.Stream stream)
        {
            System.Int64 len = stream.Length;
            System.UInt32 size = (len < this._windowSize) ? (System.UInt32)len : this._windowSize;
            this.TrainSize = size;
            stream.Position = len - size;
            this._streamPos = this._pos = 0;
            while (size > 0)
            {
                System.UInt32 curSize = this._windowSize - this._pos;
                if (size < curSize)
                    curSize = size;
                System.Int32 numReadBytes = stream.Read(this._buffer, (System.Int32)this._pos, (System.Int32)curSize);
                if (numReadBytes == 0)
                    return false;
                size -= (System.UInt32)numReadBytes;
                this._pos += (System.UInt32)numReadBytes;
                this._streamPos += (System.UInt32)numReadBytes;
                if (this._pos == this._windowSize)
                    this._streamPos = this._pos = 0;
            }
            return true;
        }

        public void ReleaseStream()
        {
            this.Flush();
            this._stream = null;
        }

        public void Flush()
        {
            System.UInt32 size = this._pos - this._streamPos;
            if (size == 0)
                return;
            this._stream.Write(this._buffer, (System.Int32)this._streamPos, (System.Int32)size);
            if (this._pos >= this._windowSize)
                this._pos = 0;
            this._streamPos = this._pos;
        }

        public void CopyBlock(System.UInt32 distance, System.UInt32 len)
        {
            System.UInt32 pos = this._pos - distance - 1;
            if (pos >= this._windowSize)
                pos += this._windowSize;
            for (; len > 0; len--)
            {
                if (pos >= this._windowSize)
                    pos = 0;
                this._buffer[this._pos++] = this._buffer[pos++];
                if (this._pos >= this._windowSize)
                    this.Flush();
            }
        }

        public void PutByte(System.Byte b)
        {
            this._buffer[this._pos++] = b;
            if (this._pos >= this._windowSize)
                this.Flush();
        }

        public System.Byte GetByte(System.UInt32 distance)
        {
            System.UInt32 pos = this._pos - distance - 1;
            if (pos >= this._windowSize)
                pos += this._windowSize;
            return this._buffer[pos];
        }
    }
}