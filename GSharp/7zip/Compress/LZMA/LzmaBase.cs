// LzmaBase.cs

namespace SevenZip.Compression.LZMA
{
    internal abstract class Base
    {
        public const System.UInt32 kNumRepDistances = 4;
        public const System.UInt32 kNumStates = 12;

        // static byte []kLiteralNextStates  = {0, 0, 0, 0, 1, 2, 3, 4,  5,  6,   4, 5};
        // static byte []kMatchNextStates    = {7, 7, 7, 7, 7, 7, 7, 10, 10, 10, 10, 10};
        // static byte []kRepNextStates      = {8, 8, 8, 8, 8, 8, 8, 11, 11, 11, 11, 11};
        // static byte []kShortRepNextStates = {9, 9, 9, 9, 9, 9, 9, 11, 11, 11, 11, 11};

        public struct State
        {
            public System.UInt32 Index;
            public void Init() => this.Index = 0;
            public void UpdateChar()
            {
                if (this.Index < 4) this.Index = 0;
                else if (this.Index < 10) this.Index -= 3;
                else this.Index -= 6;
            }
            public void UpdateMatch() => this.Index = (System.UInt32)(this.Index < 7 ? 7 : 10);
            public void UpdateRep() => this.Index = (System.UInt32)(this.Index < 7 ? 8 : 11);
            public void UpdateShortRep() => this.Index = (System.UInt32)(this.Index < 7 ? 9 : 11);
            public System.Boolean IsCharState() => this.Index < 7;
        }

        public const System.Int32 kNumPosSlotBits = 6;
        public const System.Int32 kDicLogSizeMin = 0;
        // public const int kDicLogSizeMax = 30;
        // public const uint kDistTableSizeMax = kDicLogSizeMax * 2;

        public const System.Int32 kNumLenToPosStatesBits = 2; // it's for speed optimization
        public const System.UInt32 kNumLenToPosStates = 1 << kNumLenToPosStatesBits;

        public const System.UInt32 kMatchMinLen = 2;

        public static System.UInt32 GetLenToPosState(System.UInt32 len)
        {
            len -= kMatchMinLen;
            return len < kNumLenToPosStates ? len : kNumLenToPosStates - 1;
        }

        public const System.Int32 kNumAlignBits = 4;
        public const System.UInt32 kAlignTableSize = 1 << kNumAlignBits;
        public const System.UInt32 kAlignMask = kAlignTableSize - 1;

        public const System.UInt32 kStartPosModelIndex = 4;
        public const System.UInt32 kEndPosModelIndex = 14;
        public const System.UInt32 kNumPosModels = kEndPosModelIndex - kStartPosModelIndex;

        public const System.UInt32 kNumFullDistances = 1 << ((System.Int32)kEndPosModelIndex / 2);

        public const System.UInt32 kNumLitPosStatesBitsEncodingMax = 4;
        public const System.UInt32 kNumLitContextBitsMax = 8;

        public const System.Int32 kNumPosStatesBitsMax = 4;
        public const System.UInt32 kNumPosStatesMax = 1 << kNumPosStatesBitsMax;
        public const System.Int32 kNumPosStatesBitsEncodingMax = 4;
        public const System.UInt32 kNumPosStatesEncodingMax = 1 << kNumPosStatesBitsEncodingMax;

        public const System.Int32 kNumLowLenBits = 3;
        public const System.Int32 kNumMidLenBits = 3;
        public const System.Int32 kNumHighLenBits = 8;
        public const System.UInt32 kNumLowLenSymbols = 1 << kNumLowLenBits;
        public const System.UInt32 kNumMidLenSymbols = 1 << kNumMidLenBits;
        public const System.UInt32 kNumLenSymbols = kNumLowLenSymbols + kNumMidLenSymbols +
                (1 << kNumHighLenBits);
        public const System.UInt32 kMatchMaxLen = kMatchMinLen + kNumLenSymbols - 1;
    }
}
