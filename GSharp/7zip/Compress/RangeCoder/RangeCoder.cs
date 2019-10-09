using System;

namespace SevenZip.Compression.RangeCoder
{
    internal class Decoder
    {
        public const UInt32 kTopValue = 1 << 24;
        public UInt32 Code;
        public UInt32 Range;

        // public Buffer.InBuffer Stream = new Buffer.InBuffer(1 << 16);
        public System.IO.Stream Stream;

        public void CloseStream() => this.Stream.Close();

#pragma warning disable IDE0060 // Remove unused parameter, public domain, Ignoring!

        public void Decode(UInt32 start, UInt32 size, UInt32 total)
#pragma warning restore IDE0060
        {
            this.Code -= start * this.Range;
            this.Range *= size;
            this.Normalize();
        }

        public UInt32 DecodeBit(UInt32 size0, Int32 numTotalBits)
        {
            UInt32 newBound = (this.Range >> numTotalBits) * size0;
            UInt32 symbol;
            if (this.Code < newBound)
            {
                symbol = 0;
                this.Range = newBound;
            }
            else
            {
                symbol = 1;
                this.Code -= newBound;
                this.Range -= newBound;
            }
            this.Normalize();
            return symbol;
        }

        public UInt32 DecodeDirectBits(Int32 numTotalBits)
        {
            UInt32 range = this.Range;
            UInt32 code = this.Code;
            UInt32 result = 0;
            for (Int32 i = numTotalBits; i > 0; i--)
            {
                range >>= 1;
                /*
				result <<= 1;
				if (code >= range)
				{
					code -= range;
					result |= 1;
				}
				*/
                UInt32 t = (code - range) >> 31;
                code -= range & (t - 1);
                result = (result << 1) | (1 - t);

                if (range < kTopValue)
                {
                    code = (code << 8) | (Byte)this.Stream.ReadByte();
                    range <<= 8;
                }
            }
            this.Range = range;
            this.Code = code;
            return result;
        }

        public UInt32 GetThreshold(UInt32 total) => this.Code / (this.Range /= total);

        public void Init(System.IO.Stream stream)
        {
            // Stream.Init(stream);
            this.Stream = stream;

            this.Code = 0;
            this.Range = 0xFFFFFFFF;
            for (Int32 i = 0; i < 5; i++)
                this.Code = (this.Code << 8) | (Byte)this.Stream.ReadByte();
        }

        public void Normalize()
        {
            while (this.Range < kTopValue)
            {
                this.Code = (this.Code << 8) | (Byte)this.Stream.ReadByte();
                this.Range <<= 8;
            }
        }

        public void Normalize2()
        {
            if (this.Range < kTopValue)
            {
                this.Code = (this.Code << 8) | (Byte)this.Stream.ReadByte();
                this.Range <<= 8;
            }
        }

        public void ReleaseStream() =>
                            // Stream.ReleaseStream();
                            this.Stream = null;

        // ulong GetProcessedSize() {return Stream.GetProcessedSize(); }
    }

    internal class Encoder
    {
        private Byte _cache;
        private UInt32 _cacheSize;
        private Int64 StartPosition;
        private System.IO.Stream Stream;
        public const UInt32 kTopValue = 1 << 24;
        public UInt64 Low;
        public UInt32 Range;

        public void CloseStream() => this.Stream.Close();

        public void Encode(UInt32 start, UInt32 size, UInt32 total)
        {
            this.Low += start * (this.Range /= total);
            this.Range *= size;
            while (this.Range < kTopValue)
            {
                this.Range <<= 8;
                this.ShiftLow();
            }
        }

        public void EncodeBit(UInt32 size0, Int32 numTotalBits, UInt32 symbol)
        {
            UInt32 newBound = (this.Range >> numTotalBits) * size0;
            if (symbol == 0)
                this.Range = newBound;
            else
            {
                this.Low += newBound;
                this.Range -= newBound;
            }
            while (this.Range < kTopValue)
            {
                this.Range <<= 8;
                this.ShiftLow();
            }
        }

        public void EncodeDirectBits(UInt32 v, Int32 numTotalBits)
        {
            for (Int32 i = numTotalBits - 1; i >= 0; i--)
            {
                this.Range >>= 1;
                if (((v >> i) & 1) == 1)
                    this.Low += this.Range;
                if (this.Range < kTopValue)
                {
                    this.Range <<= 8;
                    this.ShiftLow();
                }
            }
        }

        public void FlushData()
        {
            for (Int32 i = 0; i < 5; i++)
                this.ShiftLow();
        }

        public void FlushStream() => this.Stream.Flush();

        public Int64 GetProcessedSizeAdd() => this._cacheSize +
                this.Stream.Position - this.StartPosition + 4;

        public void Init()
        {
            this.StartPosition = this.Stream.Position;

            this.Low = 0;
            this.Range = 0xFFFFFFFF;
            this._cacheSize = 1;
            this._cache = 0;
        }

        public void ReleaseStream() => this.Stream = null;

        public void SetStream(System.IO.Stream stream) => this.Stream = stream;

        public void ShiftLow()
        {
            if ((UInt32)this.Low < 0xFF000000 || (UInt32)(this.Low >> 32) == 1)
            {
                Byte temp = this._cache;
                do
                {
                    this.Stream.WriteByte((Byte)(temp + (this.Low >> 32)));
                    temp = 0xFF;
                }
                while (--this._cacheSize != 0);
                this._cache = (Byte)(((UInt32)this.Low) >> 24);
            }
            this._cacheSize++;
            this.Low = ((UInt32)this.Low) << 8;
        }

        // (long)Stream.GetProcessedSize();
    }
}
