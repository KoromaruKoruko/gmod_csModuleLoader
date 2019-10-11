using System;

namespace SevenZip.Compression.RangeCoder
{
    internal struct BitDecoder
    {
        private const Int32 kNumMoveBits = 5;
        private UInt32 Prob;
        public const UInt32 kBitModelTotal = 1 << kNumBitModelTotalBits;
        public const Int32 kNumBitModelTotalBits = 11;

        public UInt32 Decode(RangeCoder.Decoder rangeDecoder)
        {
            UInt32 newBound = (rangeDecoder.Range >> kNumBitModelTotalBits) * this.Prob;
            if (rangeDecoder.Code < newBound)
            {
                rangeDecoder.Range = newBound;
                this.Prob += (kBitModelTotal - this.Prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (Byte)rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }
                return 0;
            }
            else
            {
                rangeDecoder.Range -= newBound;
                rangeDecoder.Code -= newBound;
                this.Prob -= (this.Prob) >> kNumMoveBits;
                if (rangeDecoder.Range < Decoder.kTopValue)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8) | (Byte)rangeDecoder.Stream.ReadByte();
                    rangeDecoder.Range <<= 8;
                }
                return 1;
            }
        }

        public void Init() => this.Prob = kBitModelTotal >> 1;

        public void UpdateModel(Int32 numMoveBits, UInt32 symbol)
        {
            if (symbol == 0)
                this.Prob += (kBitModelTotal - this.Prob) >> numMoveBits;
            else
                this.Prob -= (this.Prob) >> numMoveBits;
        }
    }

    internal struct BitEncoder
    {
        private const Int32 kNumMoveBits = 5;
        private const Int32 kNumMoveReducingBits = 2;
        private static readonly UInt32[] ProbPrices = new UInt32[kBitModelTotal >> kNumMoveReducingBits];
        private UInt32 Prob;
        public const UInt32 kBitModelTotal = 1 << kNumBitModelTotalBits;
        public const Int32 kNumBitModelTotalBits = 11;
        public const Int32 kNumBitPriceShiftBits = 6;

        static BitEncoder()
        {
            const Int32 kNumBits = kNumBitModelTotalBits - kNumMoveReducingBits;
            for (Int32 i = kNumBits - 1; i >= 0; i--)
            {
                UInt32 start = (UInt32)1 << (kNumBits - i - 1);
                UInt32 end = (UInt32)1 << (kNumBits - i);
                for (UInt32 j = start; j < end; j++)
                    ProbPrices[j] = ((UInt32)i << kNumBitPriceShiftBits) +
                        (((end - j) << kNumBitPriceShiftBits) >> (kNumBits - i - 1));
            }
        }

        public void Encode(Encoder encoder, UInt32 symbol)
        {
            // encoder.EncodeBit(Prob, kNumBitModelTotalBits, symbol);
            // UpdateModel(symbol);
            UInt32 newBound = (encoder.Range >> kNumBitModelTotalBits) * this.Prob;
            if (symbol == 0)
            {
                encoder.Range = newBound;
                this.Prob += (kBitModelTotal - this.Prob) >> kNumMoveBits;
            }
            else
            {
                encoder.Low += newBound;
                encoder.Range -= newBound;
                this.Prob -= (this.Prob) >> kNumMoveBits;
            }
            if (encoder.Range < Encoder.kTopValue)
            {
                encoder.Range <<= 8;
                encoder.ShiftLow();
            }
        }

        public UInt32 GetPrice(UInt32 symbol) => ProbPrices[(((this.Prob - symbol) ^ (-(Int32)symbol)) & (kBitModelTotal - 1)) >> kNumMoveReducingBits];

        public UInt32 GetPrice0() => ProbPrices[this.Prob >> kNumMoveReducingBits];

        public UInt32 GetPrice1() => ProbPrices[(kBitModelTotal - this.Prob) >> kNumMoveReducingBits];

        public void Init() => this.Prob = kBitModelTotal >> 1;

        public void UpdateModel(UInt32 symbol)
        {
            if (symbol == 0)
                this.Prob += (kBitModelTotal - this.Prob) >> kNumMoveBits;
            else
                this.Prob -= (this.Prob) >> kNumMoveBits;
        }
    }
}
