// LzmaEncoder.cs

using SevenZip.Compression.RangeCoder;
using System;
using System.IO;

namespace SevenZip.Compression.LZMA
{
    public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
    {
        private const Int32 kDefaultDictionaryLogSize = 22;

        private const UInt32 kIfinityPrice = 0xFFFFFFF;

        private const UInt32 kNumFastBytesDefault = 0x20;

        private const UInt32 kNumLenSpecSymbols = Base.kNumLowLenSymbols + Base.kNumMidLenSymbols;

        private const UInt32 kNumOpts = 1 << 12;

        private const Int32 kPropSize = 5;

        private static readonly Byte[] g_FastPos = new Byte[1 << 11];

        private static readonly String[] kMatchFinderIDs =
        {
            "BT2",
            "BT4",
        };

        private readonly UInt32[] _alignPrices = new UInt32[Base.kAlignTableSize];

        private readonly UInt32[] _distancesPrices = new UInt32[Base.kNumFullDistances << Base.kNumLenToPosStatesBits];

        private readonly RangeCoder.BitEncoder[] _isMatch = new RangeCoder.BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

        private readonly RangeCoder.BitEncoder[] _isRep = new RangeCoder.BitEncoder[Base.kNumStates];

        private readonly RangeCoder.BitEncoder[] _isRep0Long = new RangeCoder.BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

        private readonly RangeCoder.BitEncoder[] _isRepG0 = new RangeCoder.BitEncoder[Base.kNumStates];

        private readonly RangeCoder.BitEncoder[] _isRepG1 = new RangeCoder.BitEncoder[Base.kNumStates];

        private readonly RangeCoder.BitEncoder[] _isRepG2 = new RangeCoder.BitEncoder[Base.kNumStates];

        private readonly LenPriceTableEncoder _lenEncoder = new LenPriceTableEncoder();

        private readonly LiteralEncoder _literalEncoder = new LiteralEncoder();

        private readonly UInt32[] _matchDistances = new UInt32[(Base.kMatchMaxLen * 2) + 2];

        private readonly Optimal[] _optimum = new Optimal[kNumOpts];

        private readonly RangeCoder.BitTreeEncoder _posAlignEncoder = new RangeCoder.BitTreeEncoder(Base.kNumAlignBits);

        private readonly RangeCoder.BitEncoder[] _posEncoders = new RangeCoder.BitEncoder[Base.kNumFullDistances - Base.kEndPosModelIndex];

        private readonly RangeCoder.BitTreeEncoder[] _posSlotEncoder = new RangeCoder.BitTreeEncoder[Base.kNumLenToPosStates];

        private readonly UInt32[] _posSlotPrices = new UInt32[1 << (Base.kNumPosSlotBits + Base.kNumLenToPosStatesBits)];

        private readonly RangeCoder.Encoder _rangeEncoder = new RangeCoder.Encoder();

        private readonly UInt32[] _repDistances = new UInt32[Base.kNumRepDistances];

        private readonly LenPriceTableEncoder _repMatchLenEncoder = new LenPriceTableEncoder();

        private readonly Byte[] properties = new Byte[kPropSize];

        private readonly UInt32[] repLens = new UInt32[Base.kNumRepDistances];

        private readonly UInt32[] reps = new UInt32[Base.kNumRepDistances];

        private readonly UInt32[] tempPrices = new UInt32[Base.kNumFullDistances];

        private UInt32 _additionalOffset;

        private UInt32 _alignPriceCount;

        private UInt32 _dictionarySize = 1 << kDefaultDictionaryLogSize;

        private UInt32 _dictionarySizePrev = 0xFFFFFFFF;

        private UInt32 _distTableSize = kDefaultDictionaryLogSize * 2;

        private Boolean _finished;

        private Stream _inStream;

        private UInt32 _longestMatchLength;

        private Boolean _longestMatchWasFound;

        private LZ.IMatchFinder _matchFinder = null;

        private EMatchFinderType _matchFinderType = EMatchFinderType.BT4;

        private UInt32 _matchPriceCount;

        private Boolean _needReleaseMFStream;

        private UInt32 _numDistancePairs;

        private UInt32 _numFastBytes = kNumFastBytesDefault;

        private UInt32 _numFastBytesPrev = 0xFFFFFFFF;

        private Int32 _numLiteralContextBits = 3;

        private Int32 _numLiteralPosStateBits = 0;

        private UInt32 _optimumCurrentIndex;

        private UInt32 _optimumEndIndex;

        private Int32 _posStateBits = 2;

        private UInt32 _posStateMask = 4 - 1;

        private Byte _previousByte;

        private Base.State _state = new Base.State();

        private UInt32 _trainSize = 0;

        private Boolean _writeEndMark = false;

        private Int64 nowPos64;

        static Encoder()
        {
            const Byte kFastSlots = 22;
            Int32 c = 2;
            g_FastPos[0] = 0;
            g_FastPos[1] = 1;
            for (Byte slotFast = 2; slotFast < kFastSlots; slotFast++)
            {
                UInt32 k = (UInt32)1 << ((slotFast >> 1) - 1);
                for (UInt32 j = 0; j < k; j++, c++)
                    g_FastPos[c] = slotFast;
            }
        }

        public Encoder()
        {
            for (Int32 i = 0; i < kNumOpts; i++)
                this._optimum[i] = new Optimal();
            for (Int32 i = 0; i < Base.kNumLenToPosStates; i++)
                this._posSlotEncoder[i] = new RangeCoder.BitTreeEncoder(Base.kNumPosSlotBits);
        }

        private enum EMatchFinderType
        {
            BT2,
            BT4,
        };

        private static Int32 FindMatchFinder(String s)
        {
            for (Int32 m = 0; m < kMatchFinderIDs.Length; m++)
                if (s == kMatchFinderIDs[m])
                    return m;
            return -1;
        }

        private static UInt32 GetPosSlot(UInt32 pos) => pos < (1 << 11) ? g_FastPos[pos] : pos < (1 << 21) ? (UInt32)(g_FastPos[pos >> 10] + 20) : (UInt32)(g_FastPos[pos >> 20] + 40);

        private static UInt32 GetPosSlot2(UInt32 pos) => pos < (1 << 17)
                ? (UInt32)(g_FastPos[pos >> 6] + 12)
                : pos < (1 << 27) ? (UInt32)(g_FastPos[pos >> 16] + 32) : (UInt32)(g_FastPos[pos >> 26] + 52);

        private UInt32 Backward(out UInt32 backRes, UInt32 cur)
        {
            this._optimumEndIndex = cur;
            UInt32 posMem = this._optimum[cur].PosPrev;
            UInt32 backMem = this._optimum[cur].BackPrev;
            do
            {
                if (this._optimum[cur].Prev1IsChar)
                {
                    this._optimum[posMem].MakeAsChar();
                    this._optimum[posMem].PosPrev = posMem - 1;
                    if (this._optimum[cur].Prev2)
                    {
                        this._optimum[posMem - 1].Prev1IsChar = false;
                        this._optimum[posMem - 1].PosPrev = this._optimum[cur].PosPrev2;
                        this._optimum[posMem - 1].BackPrev = this._optimum[cur].BackPrev2;
                    }
                }
                UInt32 posPrev = posMem;
                UInt32 backCur = backMem;

                backMem = this._optimum[posPrev].BackPrev;
                posMem = this._optimum[posPrev].PosPrev;

                this._optimum[posPrev].BackPrev = backCur;
                this._optimum[posPrev].PosPrev = cur;
                cur = posPrev;
            }
            while (cur > 0);
            backRes = this._optimum[0].BackPrev;
            this._optimumCurrentIndex = this._optimum[0].PosPrev;
            return this._optimumCurrentIndex;
        }

        private void BaseInit()
        {
            this._state.Init();
            this._previousByte = 0;
            for (UInt32 i = 0; i < Base.kNumRepDistances; i++)
                this._repDistances[i] = 0;
        }

        private Boolean ChangePair(UInt32 smallDist, UInt32 bigDist)
        {
            const Int32 kDif = 7;
            return smallDist < ((UInt32)1 << (32 - kDif)) && bigDist >= (smallDist << kDif);
        }

        private void Create()
        {
            if (this._matchFinder == null)
            {
                LZ.BinTree bt = new LZ.BinTree();
                Int32 numHashBytes = 4;
                if (this._matchFinderType == EMatchFinderType.BT2)
                    numHashBytes = 2;
                bt.SetType(numHashBytes);
                this._matchFinder = bt;
            }
            this._literalEncoder.Create(this._numLiteralPosStateBits, this._numLiteralContextBits);

            if (this._dictionarySize == this._dictionarySizePrev && this._numFastBytesPrev == this._numFastBytes)
                return;
            this._matchFinder.Create(this._dictionarySize, kNumOpts, this._numFastBytes, Base.kMatchMaxLen + 1);
            this._dictionarySizePrev = this._dictionarySize;
            this._numFastBytesPrev = this._numFastBytes;
        }

        private void FillAlignPrices()
        {
            for (UInt32 i = 0; i < Base.kAlignTableSize; i++)
                this._alignPrices[i] = this._posAlignEncoder.ReverseGetPrice(i);
            this._alignPriceCount = 0;
        }

        private void FillDistancesPrices()
        {
            for (UInt32 i = Base.kStartPosModelIndex; i < Base.kNumFullDistances; i++)
            {
                UInt32 posSlot = GetPosSlot(i);
                Int32 footerBits = (Int32)((posSlot >> 1) - 1);
                UInt32 baseVal = (2 | (posSlot & 1)) << footerBits;
                this.tempPrices[i] = BitTreeEncoder.ReverseGetPrice(this._posEncoders,
                    baseVal - posSlot - 1, footerBits, i - baseVal);
            }

            for (UInt32 lenToPosState = 0; lenToPosState < Base.kNumLenToPosStates; lenToPosState++)
            {
                UInt32 posSlot;
                RangeCoder.BitTreeEncoder encoder = this._posSlotEncoder[lenToPosState];

                UInt32 st = lenToPosState << Base.kNumPosSlotBits;
                for (posSlot = 0; posSlot < this._distTableSize; posSlot++)
                    this._posSlotPrices[st + posSlot] = encoder.GetPrice(posSlot);
                for (posSlot = Base.kEndPosModelIndex; posSlot < this._distTableSize; posSlot++)
                    this._posSlotPrices[st + posSlot] += ((posSlot >> 1) - 1 - Base.kNumAlignBits) << RangeCoder.BitEncoder.kNumBitPriceShiftBits;

                UInt32 st2 = lenToPosState * Base.kNumFullDistances;
                UInt32 i;
                for (i = 0; i < Base.kStartPosModelIndex; i++)
                    this._distancesPrices[st2 + i] = this._posSlotPrices[st + i];
                for (; i < Base.kNumFullDistances; i++)
                    this._distancesPrices[st2 + i] = this._posSlotPrices[st + GetPosSlot(i)] + this.tempPrices[i];
            }
            this._matchPriceCount = 0;
        }

        private void Flush(UInt32 nowPos)
        {
            this.ReleaseMFStream();
            this.WriteEndMarker(nowPos & this._posStateMask);
            this._rangeEncoder.FlushData();
            this._rangeEncoder.FlushStream();
        }

        private UInt32 GetOptimum(UInt32 position, out UInt32 backRes)
        {
            if (this._optimumEndIndex != this._optimumCurrentIndex)
            {
                UInt32 lenRes = this._optimum[this._optimumCurrentIndex].PosPrev - this._optimumCurrentIndex;
                backRes = this._optimum[this._optimumCurrentIndex].BackPrev;
                this._optimumCurrentIndex = this._optimum[this._optimumCurrentIndex].PosPrev;
                return lenRes;
            }
            this._optimumCurrentIndex = this._optimumEndIndex = 0;

            UInt32 lenMain, numDistancePairs;
            if (!this._longestMatchWasFound)
            {
                this.ReadMatchDistances(out lenMain, out numDistancePairs);
            }
            else
            {
                lenMain = this._longestMatchLength;
                numDistancePairs = this._numDistancePairs;
                this._longestMatchWasFound = false;
            }

            UInt32 numAvailableBytes = this._matchFinder.GetNumAvailableBytes() + 1;
            if (numAvailableBytes < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }
            if (numAvailableBytes > Base.kMatchMaxLen)
                numAvailableBytes = Base.kMatchMaxLen;

            UInt32 repMaxIndex = 0;
            UInt32 i;
            for (i = 0; i < Base.kNumRepDistances; i++)
            {
                this.reps[i] = this._repDistances[i];
                this.repLens[i] = this._matchFinder.GetMatchLen(0 - 1, this.reps[i], Base.kMatchMaxLen);
                if (this.repLens[i] > this.repLens[repMaxIndex])
                    repMaxIndex = i;
            }
            if (this.repLens[repMaxIndex] >= this._numFastBytes)
            {
                backRes = repMaxIndex;
                UInt32 lenRes = this.repLens[repMaxIndex];
                this.MovePos(lenRes - 1);
                return lenRes;
            }

            if (lenMain >= this._numFastBytes)
            {
                backRes = this._matchDistances[numDistancePairs - 1] + Base.kNumRepDistances;
                this.MovePos(lenMain - 1);
                return lenMain;
            }

            Byte currentByte = this._matchFinder.GetIndexByte(0 - 1);
            Byte matchByte = this._matchFinder.GetIndexByte((Int32)(0 - this._repDistances[0] - 1 - 1));

            if (lenMain < 2 && currentByte != matchByte && this.repLens[repMaxIndex] < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }

            this._optimum[0].State = this._state;

            UInt32 posState = position & this._posStateMask;

            this._optimum[1].Price = this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
                    this._literalEncoder.GetSubCoder(position, this._previousByte).GetPrice(!this._state.IsCharState(), matchByte, currentByte);
            this._optimum[1].MakeAsChar();

            UInt32 matchPrice = this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            UInt32 repMatchPrice = matchPrice + this._isRep[this._state.Index].GetPrice1();

            if (matchByte == currentByte)
            {
                UInt32 shortRepPrice = repMatchPrice + this.GetRepLen1Price(this._state, posState);
                if (shortRepPrice < this._optimum[1].Price)
                {
                    this._optimum[1].Price = shortRepPrice;
                    this._optimum[1].MakeAsShortRep();
                }
            }

            UInt32 lenEnd = (lenMain >= this.repLens[repMaxIndex]) ? lenMain : this.repLens[repMaxIndex];

            if (lenEnd < 2)
            {
                backRes = this._optimum[1].BackPrev;
                return 1;
            }

            this._optimum[1].PosPrev = 0;

            this._optimum[0].Backs0 = this.reps[0];
            this._optimum[0].Backs1 = this.reps[1];
            this._optimum[0].Backs2 = this.reps[2];
            this._optimum[0].Backs3 = this.reps[3];

            UInt32 len = lenEnd;
            do
                this._optimum[len--].Price = kIfinityPrice;
            while (len >= 2);

            for (i = 0; i < Base.kNumRepDistances; i++)
            {
                UInt32 repLen = this.repLens[i];
                if (repLen < 2)
                    continue;
                UInt32 price = repMatchPrice + this.GetPureRepPrice(i, this._state, posState);
                do
                {
                    UInt32 curAndLenPrice = price + this._repMatchLenEncoder.GetPrice(repLen - 2, posState);
                    Optimal optimum = this._optimum[repLen];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = i;
                        optimum.Prev1IsChar = false;
                    }
                }
                while (--repLen >= 2);
            }

            UInt32 normalMatchPrice = matchPrice + this._isRep[this._state.Index].GetPrice0();

            len = (this.repLens[0] >= 2) ? this.repLens[0] + 1 : 2;
            if (len <= lenMain)
            {
                UInt32 offs = 0;
                while (len > this._matchDistances[offs])
                    offs += 2;
                for (; ; len++)
                {
                    UInt32 distance = this._matchDistances[offs + 1];
                    UInt32 curAndLenPrice = normalMatchPrice + this.GetPosLenPrice(distance, len, posState);
                    Optimal optimum = this._optimum[len];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = distance + Base.kNumRepDistances;
                        optimum.Prev1IsChar = false;
                    }
                    if (len == this._matchDistances[offs])
                    {
                        offs += 2;
                        if (offs == numDistancePairs)
                            break;
                    }
                }
            }

            UInt32 cur = 0;

            while (true)
            {
                cur++;
                if (cur == lenEnd)
                    return this.Backward(out backRes, cur);
                this.ReadMatchDistances(out UInt32 newLen, out numDistancePairs);
                if (newLen >= this._numFastBytes)
                {
                    this._numDistancePairs = numDistancePairs;
                    this._longestMatchLength = newLen;
                    this._longestMatchWasFound = true;
                    return this.Backward(out backRes, cur);
                }
                position++;
                UInt32 posPrev = this._optimum[cur].PosPrev;
                Base.State state;
                if (this._optimum[cur].Prev1IsChar)
                {
                    posPrev--;
                    if (this._optimum[cur].Prev2)
                    {
                        state = this._optimum[this._optimum[cur].PosPrev2].State;
                        if (this._optimum[cur].BackPrev2 < Base.kNumRepDistances)
                            state.UpdateRep();
                        else
                            state.UpdateMatch();
                    }
                    else
                        state = this._optimum[posPrev].State;
                    state.UpdateChar();
                }
                else
                    state = this._optimum[posPrev].State;
                if (posPrev == cur - 1)
                {
                    if (this._optimum[cur].IsShortRep())
                        state.UpdateShortRep();
                    else
                        state.UpdateChar();
                }
                else
                {
                    UInt32 pos;
                    if (this._optimum[cur].Prev1IsChar && this._optimum[cur].Prev2)
                    {
                        posPrev = this._optimum[cur].PosPrev2;
                        pos = this._optimum[cur].BackPrev2;
                        state.UpdateRep();
                    }
                    else
                    {
                        pos = this._optimum[cur].BackPrev;
                        if (pos < Base.kNumRepDistances)
                            state.UpdateRep();
                        else
                            state.UpdateMatch();
                    }
                    Optimal opt = this._optimum[posPrev];
                    if (pos < Base.kNumRepDistances)
                    {
                        if (pos == 0)
                        {
                            this.reps[0] = opt.Backs0;
                            this.reps[1] = opt.Backs1;
                            this.reps[2] = opt.Backs2;
                            this.reps[3] = opt.Backs3;
                        }
                        else if (pos == 1)
                        {
                            this.reps[0] = opt.Backs1;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs2;
                            this.reps[3] = opt.Backs3;
                        }
                        else if (pos == 2)
                        {
                            this.reps[0] = opt.Backs2;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs1;
                            this.reps[3] = opt.Backs3;
                        }
                        else
                        {
                            this.reps[0] = opt.Backs3;
                            this.reps[1] = opt.Backs0;
                            this.reps[2] = opt.Backs1;
                            this.reps[3] = opt.Backs2;
                        }
                    }
                    else
                    {
                        this.reps[0] = pos - Base.kNumRepDistances;
                        this.reps[1] = opt.Backs0;
                        this.reps[2] = opt.Backs1;
                        this.reps[3] = opt.Backs2;
                    }
                }
                this._optimum[cur].State = state;
                this._optimum[cur].Backs0 = this.reps[0];
                this._optimum[cur].Backs1 = this.reps[1];
                this._optimum[cur].Backs2 = this.reps[2];
                this._optimum[cur].Backs3 = this.reps[3];
                UInt32 curPrice = this._optimum[cur].Price;

                currentByte = this._matchFinder.GetIndexByte(0 - 1);
                matchByte = this._matchFinder.GetIndexByte((Int32)(0 - this.reps[0] - 1 - 1));

                posState = position & this._posStateMask;

                UInt32 curAnd1Price = curPrice +
                    this._isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
                    this._literalEncoder.GetSubCoder(position, this._matchFinder.GetIndexByte(0 - 2)).
                    GetPrice(!state.IsCharState(), matchByte, currentByte);

                Optimal nextOptimum = this._optimum[cur + 1];

                Boolean nextIsChar = false;
                if (curAnd1Price < nextOptimum.Price)
                {
                    nextOptimum.Price = curAnd1Price;
                    nextOptimum.PosPrev = cur;
                    nextOptimum.MakeAsChar();
                    nextIsChar = true;
                }

                matchPrice = curPrice + this._isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
                repMatchPrice = matchPrice + this._isRep[state.Index].GetPrice1();

                if (matchByte == currentByte &&
                    !(nextOptimum.PosPrev < cur && nextOptimum.BackPrev == 0))
                {
                    UInt32 shortRepPrice = repMatchPrice + this.GetRepLen1Price(state, posState);
                    if (shortRepPrice <= nextOptimum.Price)
                    {
                        nextOptimum.Price = shortRepPrice;
                        nextOptimum.PosPrev = cur;
                        nextOptimum.MakeAsShortRep();
                        nextIsChar = true;
                    }
                }

                UInt32 numAvailableBytesFull = this._matchFinder.GetNumAvailableBytes() + 1;
                numAvailableBytesFull = Math.Min(kNumOpts - 1 - cur, numAvailableBytesFull);
                numAvailableBytes = numAvailableBytesFull;

                if (numAvailableBytes < 2)
                    continue;
                if (numAvailableBytes > this._numFastBytes)
                    numAvailableBytes = this._numFastBytes;
                if (!nextIsChar && matchByte != currentByte)
                {
                    // try Literal + rep0
                    UInt32 t = Math.Min(numAvailableBytesFull - 1, this._numFastBytes);
                    UInt32 lenTest2 = this._matchFinder.GetMatchLen(0, this.reps[0], t);
                    if (lenTest2 >= 2)
                    {
                        Base.State state2 = state;
                        state2.UpdateChar();
                        UInt32 posStateNext = (position + 1) & this._posStateMask;
                        UInt32 nextRepMatchPrice = curAnd1Price +
                            this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1() +
                            this._isRep[state2.Index].GetPrice1();
                        {
                            UInt32 offset = cur + 1 + lenTest2;
                            while (lenEnd < offset)
                                this._optimum[++lenEnd].Price = kIfinityPrice;
                            UInt32 curAndLenPrice = nextRepMatchPrice + this.GetRepPrice(
                                0, lenTest2, state2, posStateNext);
                            Optimal optimum = this._optimum[offset];
                            if (curAndLenPrice < optimum.Price)
                            {
                                optimum.Price = curAndLenPrice;
                                optimum.PosPrev = cur + 1;
                                optimum.BackPrev = 0;
                                optimum.Prev1IsChar = true;
                                optimum.Prev2 = false;
                            }
                        }
                    }
                }

                UInt32 startLen = 2; // speed optimization

                for (UInt32 repIndex = 0; repIndex < Base.kNumRepDistances; repIndex++)
                {
                    UInt32 lenTest = this._matchFinder.GetMatchLen(0 - 1, this.reps[repIndex], numAvailableBytes);
                    if (lenTest < 2)
                        continue;
                    UInt32 lenTestTemp = lenTest;
                    do
                    {
                        while (lenEnd < cur + lenTest)
                            this._optimum[++lenEnd].Price = kIfinityPrice;
                        UInt32 curAndLenPrice = repMatchPrice + this.GetRepPrice(repIndex, lenTest, state, posState);
                        Optimal optimum = this._optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = repIndex;
                            optimum.Prev1IsChar = false;
                        }
                    }
                    while (--lenTest >= 2);
                    lenTest = lenTestTemp;

                    if (repIndex == 0)
                        startLen = lenTest + 1;

                    // if (_maxMode)
                    if (lenTest < numAvailableBytesFull)
                    {
                        UInt32 t = Math.Min(numAvailableBytesFull - 1 - lenTest, this._numFastBytes);
                        UInt32 lenTest2 = this._matchFinder.GetMatchLen((Int32)lenTest, this.reps[repIndex], t);
                        if (lenTest2 >= 2)
                        {
                            Base.State state2 = state;
                            state2.UpdateRep();
                            UInt32 posStateNext = (position + lenTest) & this._posStateMask;
                            UInt32 curAndLenCharPrice =
                                    repMatchPrice + this.GetRepPrice(repIndex, lenTest, state, posState) +
                                    this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice0() +
                                    this._literalEncoder.GetSubCoder(position + lenTest,
                                    this._matchFinder.GetIndexByte((Int32)lenTest - 1 - 1)).GetPrice(true,
                                    this._matchFinder.GetIndexByte((Int32)lenTest - 1 - (Int32)(this.reps[repIndex] + 1)),
                                    this._matchFinder.GetIndexByte((Int32)lenTest - 1));
                            state2.UpdateChar();
                            posStateNext = (position + lenTest + 1) & this._posStateMask;
                            UInt32 nextMatchPrice = curAndLenCharPrice + this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1();
                            UInt32 nextRepMatchPrice = nextMatchPrice + this._isRep[state2.Index].GetPrice1();

                            // for(; lenTest2 >= 2; lenTest2--)
                            {
                                UInt32 offset = lenTest + 1 + lenTest2;
                                while (lenEnd < cur + offset)
                                    this._optimum[++lenEnd].Price = kIfinityPrice;
                                UInt32 curAndLenPrice = nextRepMatchPrice + this.GetRepPrice(0, lenTest2, state2, posStateNext);
                                Optimal optimum = this._optimum[cur + offset];
                                if (curAndLenPrice < optimum.Price)
                                {
                                    optimum.Price = curAndLenPrice;
                                    optimum.PosPrev = cur + lenTest + 1;
                                    optimum.BackPrev = 0;
                                    optimum.Prev1IsChar = true;
                                    optimum.Prev2 = true;
                                    optimum.PosPrev2 = cur;
                                    optimum.BackPrev2 = repIndex;
                                }
                            }
                        }
                    }
                }

                if (newLen > numAvailableBytes)
                {
                    newLen = numAvailableBytes;
                    for (numDistancePairs = 0; newLen > this._matchDistances[numDistancePairs]; numDistancePairs += 2) ;
                    this._matchDistances[numDistancePairs] = newLen;
                    numDistancePairs += 2;
                }
                if (newLen >= startLen)
                {
                    normalMatchPrice = matchPrice + this._isRep[state.Index].GetPrice0();
                    while (lenEnd < cur + newLen)
                        this._optimum[++lenEnd].Price = kIfinityPrice;

                    UInt32 offs = 0;
                    while (startLen > this._matchDistances[offs])
                        offs += 2;

                    for (UInt32 lenTest = startLen; ; lenTest++)
                    {
                        UInt32 curBack = this._matchDistances[offs + 1];
                        UInt32 curAndLenPrice = normalMatchPrice + this.GetPosLenPrice(curBack, lenTest, posState);
                        Optimal optimum = this._optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = curBack + Base.kNumRepDistances;
                            optimum.Prev1IsChar = false;
                        }

                        if (lenTest == this._matchDistances[offs])
                        {
                            if (lenTest < numAvailableBytesFull)
                            {
                                UInt32 t = Math.Min(numAvailableBytesFull - 1 - lenTest, this._numFastBytes);
                                UInt32 lenTest2 = this._matchFinder.GetMatchLen((Int32)lenTest, curBack, t);
                                if (lenTest2 >= 2)
                                {
                                    Base.State state2 = state;
                                    state2.UpdateMatch();
                                    UInt32 posStateNext = (position + lenTest) & this._posStateMask;
                                    UInt32 curAndLenCharPrice = curAndLenPrice +
                                        this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice0() +
                                        this._literalEncoder.GetSubCoder(position + lenTest,
                                        this._matchFinder.GetIndexByte((Int32)lenTest - 1 - 1)).
                                        GetPrice(true,
                                        this._matchFinder.GetIndexByte((Int32)lenTest - (Int32)(curBack + 1) - 1),
                                        this._matchFinder.GetIndexByte((Int32)lenTest - 1));
                                    state2.UpdateChar();
                                    posStateNext = (position + lenTest + 1) & this._posStateMask;
                                    UInt32 nextMatchPrice = curAndLenCharPrice + this._isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1();
                                    UInt32 nextRepMatchPrice = nextMatchPrice + this._isRep[state2.Index].GetPrice1();

                                    UInt32 offset = lenTest + 1 + lenTest2;
                                    while (lenEnd < cur + offset)
                                        this._optimum[++lenEnd].Price = kIfinityPrice;
                                    curAndLenPrice = nextRepMatchPrice + this.GetRepPrice(0, lenTest2, state2, posStateNext);
                                    optimum = this._optimum[cur + offset];
                                    if (curAndLenPrice < optimum.Price)
                                    {
                                        optimum.Price = curAndLenPrice;
                                        optimum.PosPrev = cur + lenTest + 1;
                                        optimum.BackPrev = 0;
                                        optimum.Prev1IsChar = true;
                                        optimum.Prev2 = true;
                                        optimum.PosPrev2 = cur;
                                        optimum.BackPrev2 = curBack + Base.kNumRepDistances;
                                    }
                                }
                            }
                            offs += 2;
                            if (offs == numDistancePairs)
                                break;
                        }
                    }
                }
            }
        }

        private UInt32 GetPosLenPrice(UInt32 pos, UInt32 len, UInt32 posState)
        {
            UInt32 price;
            UInt32 lenToPosState = Base.GetLenToPosState(len);
            price = pos < Base.kNumFullDistances
                ? this._distancesPrices[(lenToPosState * Base.kNumFullDistances) + pos]
                : this._posSlotPrices[(lenToPosState << Base.kNumPosSlotBits) + GetPosSlot2(pos)] +
                    this._alignPrices[pos & Base.kAlignMask];
            return price + this._lenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
        }

        private UInt32 GetPureRepPrice(UInt32 repIndex, Base.State state, UInt32 posState)
        {
            UInt32 price;
            if (repIndex == 0)
            {
                price = this._isRepG0[state.Index].GetPrice0();
                price += this._isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
            }
            else
            {
                price = this._isRepG0[state.Index].GetPrice1();
                if (repIndex == 1)
                    price += this._isRepG1[state.Index].GetPrice0();
                else
                {
                    price += this._isRepG1[state.Index].GetPrice1();
                    price += this._isRepG2[state.Index].GetPrice(repIndex - 2);
                }
            }
            return price;
        }

        private UInt32 GetRepLen1Price(Base.State state, UInt32 posState) => this._isRepG0[state.Index].GetPrice0() +
                    this._isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0();

        private UInt32 GetRepPrice(UInt32 repIndex, UInt32 len, Base.State state, UInt32 posState)
        {
            UInt32 price = this._repMatchLenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
            return price + this.GetPureRepPrice(repIndex, state, posState);
        }

        private void Init()
        {
            this.BaseInit();
            this._rangeEncoder.Init();

            UInt32 i;
            for (i = 0; i < Base.kNumStates; i++)
            {
                for (UInt32 j = 0; j <= this._posStateMask; j++)
                {
                    UInt32 complexState = (i << Base.kNumPosStatesBitsMax) + j;
                    this._isMatch[complexState].Init();
                    this._isRep0Long[complexState].Init();
                }
                this._isRep[i].Init();
                this._isRepG0[i].Init();
                this._isRepG1[i].Init();
                this._isRepG2[i].Init();
            }
            this._literalEncoder.Init();
            for (i = 0; i < Base.kNumLenToPosStates; i++)
                this._posSlotEncoder[i].Init();
            for (i = 0; i < Base.kNumFullDistances - Base.kEndPosModelIndex; i++)
                this._posEncoders[i].Init();

            this._lenEncoder.Init((UInt32)1 << this._posStateBits);
            this._repMatchLenEncoder.Init((UInt32)1 << this._posStateBits);

            this._posAlignEncoder.Init();

            this._longestMatchWasFound = false;
            this._optimumEndIndex = 0;
            this._optimumCurrentIndex = 0;
            this._additionalOffset = 0;
        }

        private void MovePos(UInt32 num)
        {
            if (num > 0)
            {
                this._matchFinder.Skip(num);
                this._additionalOffset += num;
            }
        }

        private void ReadMatchDistances(out UInt32 lenRes, out UInt32 numDistancePairs)
        {
            lenRes = 0;
            numDistancePairs = this._matchFinder.GetMatches(this._matchDistances);
            if (numDistancePairs > 0)
            {
                lenRes = this._matchDistances[numDistancePairs - 2];
                if (lenRes == this._numFastBytes)
                    lenRes += this._matchFinder.GetMatchLen((Int32)lenRes - 1, this._matchDistances[numDistancePairs - 1],
                        Base.kMatchMaxLen - lenRes);
            }
            this._additionalOffset++;
        }

        private void ReleaseMFStream()
        {
            if (this._matchFinder != null && this._needReleaseMFStream)
            {
                this._matchFinder.ReleaseStream();
                this._needReleaseMFStream = false;
            }
        }

        private void ReleaseOutStream() => this._rangeEncoder.ReleaseStream();

        private void ReleaseStreams()
        {
            this.ReleaseMFStream();
            this.ReleaseOutStream();
        }

        private void SetOutStream(Stream outStream) => this._rangeEncoder.SetStream(outStream);

        private void SetStreams(Stream inStream, Stream outStream,
#pragma warning disable IDE0060 // Remove unused parameter SYNTAXER:: Implicit function, ignoring rule!
                Int64 inSize, Int64 outSize)
#pragma warning restore IDE0060
        {
            this._inStream = inStream;
            this._finished = false;
            this.Create();
            this.SetOutStream(outStream);
            this.Init();

            // if (!_fastMode)
            {
                this.FillDistancesPrices();
                this.FillAlignPrices();
            }

            this._lenEncoder.SetTableSize(this._numFastBytes + 1 - Base.kMatchMinLen);
            this._lenEncoder.UpdateTables((UInt32)1 << this._posStateBits);
            this._repMatchLenEncoder.SetTableSize(this._numFastBytes + 1 - Base.kMatchMinLen);
            this._repMatchLenEncoder.UpdateTables((UInt32)1 << this._posStateBits);

            this.nowPos64 = 0;
        }

        private void SetWriteEndMarkerMode(Boolean writeEndMarker) => this._writeEndMark = writeEndMarker;

        private void WriteEndMarker(UInt32 posState)
        {
            if (!this._writeEndMark)
                return;

            this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(this._rangeEncoder, 1);
            this._isRep[this._state.Index].Encode(this._rangeEncoder, 0);
            this._state.UpdateMatch();
            UInt32 len = Base.kMatchMinLen;
            this._lenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
            UInt32 posSlot = (1 << Base.kNumPosSlotBits) - 1;
            UInt32 lenToPosState = Base.GetLenToPosState(len);
            this._posSlotEncoder[lenToPosState].Encode(this._rangeEncoder, posSlot);
            Int32 footerBits = 30;
            UInt32 posReduced = (((UInt32)1) << footerBits) - 1;
            this._rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
            this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & Base.kAlignMask);
        }

        public void Code(Stream inStream, Stream outStream,
            Int64 inSize, Int64 outSize, ICodeProgress progress)
        {
            this._needReleaseMFStream = false;
            try
            {
                this.SetStreams(inStream, outStream, inSize, outSize);
                while (true)
                {
                    this.CodeOneBlock(out Int64 processedInSize, out Int64 processedOutSize, out Boolean finished);
                    if (finished)
                        return;
                    if (progress != null)
                    {
                        progress.SetProgress(processedInSize, processedOutSize);
                    }
                }
            }
            finally
            {
                this.ReleaseStreams();
            }
        }

        public void CodeOneBlock(out Int64 inSize, out Int64 outSize, out Boolean finished)
        {
            inSize = 0;
            outSize = 0;
            finished = true;

            if (this._inStream != null)
            {
                this._matchFinder.SetStream(this._inStream);
                this._matchFinder.Init();
                this._needReleaseMFStream = true;
                this._inStream = null;
                if (this._trainSize > 0)
                    this._matchFinder.Skip(this._trainSize);
            }

            if (this._finished)
                return;
            this._finished = true;

            Int64 progressPosValuePrev = this.nowPos64;
            if (this.nowPos64 == 0)
            {
                if (this._matchFinder.GetNumAvailableBytes() == 0)
                {
                    this.Flush((UInt32)this.nowPos64);
                    return;
                }
                // it's not used
                this.ReadMatchDistances(out UInt32 len, out UInt32 numDistancePairs);
                UInt32 posState = (UInt32)this.nowPos64 & this._posStateMask;
                this._isMatch[(this._state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(this._rangeEncoder, 0);
                this._state.UpdateChar();
                Byte curByte = this._matchFinder.GetIndexByte((Int32)(0 - this._additionalOffset));
                this._literalEncoder.GetSubCoder((UInt32)this.nowPos64, this._previousByte).Encode(this._rangeEncoder, curByte);
                this._previousByte = curByte;
                this._additionalOffset--;
                this.nowPos64++;
            }
            if (this._matchFinder.GetNumAvailableBytes() == 0)
            {
                this.Flush((UInt32)this.nowPos64);
                return;
            }
            while (true)
            {
                UInt32 len = this.GetOptimum((UInt32)this.nowPos64, out UInt32 pos);

                UInt32 posState = ((UInt32)this.nowPos64) & this._posStateMask;
                UInt32 complexState = (this._state.Index << Base.kNumPosStatesBitsMax) + posState;
                if (len == 1 && pos == 0xFFFFFFFF)
                {
                    this._isMatch[complexState].Encode(this._rangeEncoder, 0);
                    Byte curByte = this._matchFinder.GetIndexByte((Int32)(0 - this._additionalOffset));
                    LiteralEncoder.Encoder2 subCoder = this._literalEncoder.GetSubCoder((UInt32)this.nowPos64, this._previousByte);
                    if (!this._state.IsCharState())
                    {
                        Byte matchByte = this._matchFinder.GetIndexByte((Int32)(0 - this._repDistances[0] - 1 - this._additionalOffset));
                        subCoder.EncodeMatched(this._rangeEncoder, matchByte, curByte);
                    }
                    else
                        subCoder.Encode(this._rangeEncoder, curByte);
                    this._previousByte = curByte;
                    this._state.UpdateChar();
                }
                else
                {
                    this._isMatch[complexState].Encode(this._rangeEncoder, 1);
                    if (pos < Base.kNumRepDistances)
                    {
                        this._isRep[this._state.Index].Encode(this._rangeEncoder, 1);
                        if (pos == 0)
                        {
                            this._isRepG0[this._state.Index].Encode(this._rangeEncoder, 0);
                            if (len == 1)
                                this._isRep0Long[complexState].Encode(this._rangeEncoder, 0);
                            else
                                this._isRep0Long[complexState].Encode(this._rangeEncoder, 1);
                        }
                        else
                        {
                            this._isRepG0[this._state.Index].Encode(this._rangeEncoder, 1);
                            if (pos == 1)
                                this._isRepG1[this._state.Index].Encode(this._rangeEncoder, 0);
                            else
                            {
                                this._isRepG1[this._state.Index].Encode(this._rangeEncoder, 1);
                                this._isRepG2[this._state.Index].Encode(this._rangeEncoder, pos - 2);
                            }
                        }
                        if (len == 1)
                            this._state.UpdateShortRep();
                        else
                        {
                            this._repMatchLenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
                            this._state.UpdateRep();
                        }
                        UInt32 distance = this._repDistances[pos];
                        if (pos != 0)
                        {
                            for (UInt32 i = pos; i >= 1; i--)
                                this._repDistances[i] = this._repDistances[i - 1];
                            this._repDistances[0] = distance;
                        }
                    }
                    else
                    {
                        this._isRep[this._state.Index].Encode(this._rangeEncoder, 0);
                        this._state.UpdateMatch();
                        this._lenEncoder.Encode(this._rangeEncoder, len - Base.kMatchMinLen, posState);
                        pos -= Base.kNumRepDistances;
                        UInt32 posSlot = GetPosSlot(pos);
                        UInt32 lenToPosState = Base.GetLenToPosState(len);
                        this._posSlotEncoder[lenToPosState].Encode(this._rangeEncoder, posSlot);

                        if (posSlot >= Base.kStartPosModelIndex)
                        {
                            Int32 footerBits = (Int32)((posSlot >> 1) - 1);
                            UInt32 baseVal = (2 | (posSlot & 1)) << footerBits;
                            UInt32 posReduced = pos - baseVal;

                            if (posSlot < Base.kEndPosModelIndex)
                                RangeCoder.BitTreeEncoder.ReverseEncode(this._posEncoders,
                                        baseVal - posSlot - 1, this._rangeEncoder, footerBits, posReduced);
                            else
                            {
                                this._rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
                                this._posAlignEncoder.ReverseEncode(this._rangeEncoder, posReduced & Base.kAlignMask);
                                this._alignPriceCount++;
                            }
                        }
                        UInt32 distance = pos;
                        for (UInt32 i = Base.kNumRepDistances - 1; i >= 1; i--)
                            this._repDistances[i] = this._repDistances[i - 1];
                        this._repDistances[0] = distance;
                        this._matchPriceCount++;
                    }
                    this._previousByte = this._matchFinder.GetIndexByte((Int32)(len - 1 - this._additionalOffset));
                }
                this._additionalOffset -= len;
                this.nowPos64 += len;
                if (this._additionalOffset == 0)
                {
                    // if (!_fastMode)
                    if (this._matchPriceCount >= (1 << 7))
                        this.FillDistancesPrices();
                    if (this._alignPriceCount >= Base.kAlignTableSize)
                        this.FillAlignPrices();
                    inSize = this.nowPos64;
                    outSize = this._rangeEncoder.GetProcessedSizeAdd();
                    if (this._matchFinder.GetNumAvailableBytes() == 0)
                    {
                        this.Flush((UInt32)this.nowPos64);
                        return;
                    }

                    if (this.nowPos64 - progressPosValuePrev >= (1 << 12))
                    {
                        this._finished = false;
                        finished = false;
                        return;
                    }
                }
            }
        }

        public void SetCoderProperties(CoderPropID[] propIDs, Object[] properties)
        {
            for (UInt32 i = 0; i < properties.Length; i++)
            {
                Object prop = properties[i];
                switch (propIDs[i])
                {
                    case CoderPropID.NumFastBytes:
                    {
                        if (!(prop is Int32))
                            throw new InvalidParamException();
                        Int32 numFastBytes = (Int32)prop;
                        if (numFastBytes < 5 || numFastBytes > Base.kMatchMaxLen)
                            throw new InvalidParamException();
                        this._numFastBytes = (UInt32)numFastBytes;
                        break;
                    }
                    case CoderPropID.Algorithm:
                    {
                        /*
						if (!(prop is Int32))
							throw new InvalidParamException();
						Int32 maximize = (Int32)prop;
						_fastMode = (maximize == 0);
						_maxMode = (maximize >= 2);
						*/
                        break;
                    }
                    case CoderPropID.MatchFinder:
                    {
                        if (!(prop is String))
                            throw new InvalidParamException();
                        EMatchFinderType matchFinderIndexPrev = this._matchFinderType;
                        Int32 m = FindMatchFinder(((String)prop).ToUpper());
                        if (m < 0)
                            throw new InvalidParamException();
                        this._matchFinderType = (EMatchFinderType)m;
                        if (this._matchFinder != null && matchFinderIndexPrev != this._matchFinderType)
                        {
                            this._dictionarySizePrev = 0xFFFFFFFF;
                            this._matchFinder = null;
                        }
                        break;
                    }
                    case CoderPropID.DictionarySize:
                    {
                        const Int32 kDicLogSizeMaxCompress = 30;
                        if (!(prop is Int32))
                            throw new InvalidParamException(); ;
                        Int32 dictionarySize = (Int32)prop;
                        if (dictionarySize < (UInt32)(1 << Base.kDicLogSizeMin) ||
                            dictionarySize > (UInt32)(1 << kDicLogSizeMaxCompress))
                            throw new InvalidParamException();
                        this._dictionarySize = (UInt32)dictionarySize;
                        Int32 dicLogSize;
                        for (dicLogSize = 0; dicLogSize < (UInt32)kDicLogSizeMaxCompress; dicLogSize++)
                            if (dictionarySize <= ((UInt32)1 << dicLogSize))
                                break;
                        this._distTableSize = (UInt32)dicLogSize * 2;
                        break;
                    }
                    case CoderPropID.PosStateBits:
                    {
                        if (!(prop is Int32))
                            throw new InvalidParamException();
                        Int32 v = (Int32)prop;
                        if (v < 0 || v > (UInt32)Base.kNumPosStatesBitsEncodingMax)
                            throw new InvalidParamException();
                        this._posStateBits = v;
                        this._posStateMask = (((UInt32)1) << this._posStateBits) - 1;
                        break;
                    }
                    case CoderPropID.LitPosBits:
                    {
                        if (!(prop is Int32))
                            throw new InvalidParamException();
                        Int32 v = (Int32)prop;
                        if (v < 0 || v > Base.kNumLitPosStatesBitsEncodingMax)
                            throw new InvalidParamException();
                        this._numLiteralPosStateBits = v;
                        break;
                    }
                    case CoderPropID.LitContextBits:
                    {
                        if (!(prop is Int32))
                            throw new InvalidParamException();
                        Int32 v = (Int32)prop;
                        if (v < 0 || v > Base.kNumLitContextBitsMax)
                            throw new InvalidParamException(); ;
                        this._numLiteralContextBits = v;
                        break;
                    }
                    case CoderPropID.EndMarker:
                    {
                        if (!(prop is Boolean))
                            throw new InvalidParamException();
                        this.SetWriteEndMarkerMode((Boolean)prop);
                        break;
                    }
                    default:
                        throw new InvalidParamException();
                }
            }
        }

        public void SetTrainSize(UInt32 trainSize) => this._trainSize = trainSize;

        public void WriteCoderProperties(Stream outStream)
        {
            this.properties[0] = (Byte)((((this._posStateBits * 5) + this._numLiteralPosStateBits) * 9) + this._numLiteralContextBits);
            for (Int32 i = 0; i < 4; i++)
                this.properties[1 + i] = (Byte)((this._dictionarySize >> (8 * i)) & 0xFF);
            outStream.Write(this.properties, 0, kPropSize);
        }

        private class LenEncoder
        {
            private readonly RangeCoder.BitTreeEncoder _highCoder = new RangeCoder.BitTreeEncoder(Base.kNumHighLenBits);
            private readonly RangeCoder.BitTreeEncoder[] _lowCoder = new RangeCoder.BitTreeEncoder[Base.kNumPosStatesEncodingMax];
            private readonly RangeCoder.BitTreeEncoder[] _midCoder = new RangeCoder.BitTreeEncoder[Base.kNumPosStatesEncodingMax];
            private RangeCoder.BitEncoder _choice = new RangeCoder.BitEncoder();
            private RangeCoder.BitEncoder _choice2 = new RangeCoder.BitEncoder();

            public LenEncoder()
            {
                for (UInt32 posState = 0; posState < Base.kNumPosStatesEncodingMax; posState++)
                {
                    this._lowCoder[posState] = new RangeCoder.BitTreeEncoder(Base.kNumLowLenBits);
                    this._midCoder[posState] = new RangeCoder.BitTreeEncoder(Base.kNumMidLenBits);
                }
            }

            public void Encode(RangeCoder.Encoder rangeEncoder, UInt32 symbol, UInt32 posState)
            {
                if (symbol < Base.kNumLowLenSymbols)
                {
                    this._choice.Encode(rangeEncoder, 0);
                    this._lowCoder[posState].Encode(rangeEncoder, symbol);
                }
                else
                {
                    symbol -= Base.kNumLowLenSymbols;
                    this._choice.Encode(rangeEncoder, 1);
                    if (symbol < Base.kNumMidLenSymbols)
                    {
                        this._choice2.Encode(rangeEncoder, 0);
                        this._midCoder[posState].Encode(rangeEncoder, symbol);
                    }
                    else
                    {
                        this._choice2.Encode(rangeEncoder, 1);
                        this._highCoder.Encode(rangeEncoder, symbol - Base.kNumMidLenSymbols);
                    }
                }
            }

            public void Init(UInt32 numPosStates)
            {
                this._choice.Init();
                this._choice2.Init();
                for (UInt32 posState = 0; posState < numPosStates; posState++)
                {
                    this._lowCoder[posState].Init();
                    this._midCoder[posState].Init();
                }
                this._highCoder.Init();
            }

            public void SetPrices(UInt32 posState, UInt32 numSymbols, UInt32[] prices, UInt32 st)
            {
                UInt32 a0 = this._choice.GetPrice0();
                UInt32 a1 = this._choice.GetPrice1();
                UInt32 b0 = a1 + this._choice2.GetPrice0();
                UInt32 b1 = a1 + this._choice2.GetPrice1();
                UInt32 i = 0;
                for (i = 0; i < Base.kNumLowLenSymbols; i++)
                {
                    if (i >= numSymbols)
                        return;
                    prices[st + i] = a0 + this._lowCoder[posState].GetPrice(i);
                }
                for (; i < Base.kNumLowLenSymbols + Base.kNumMidLenSymbols; i++)
                {
                    if (i >= numSymbols)
                        return;
                    prices[st + i] = b0 + this._midCoder[posState].GetPrice(i - Base.kNumLowLenSymbols);
                }
                for (; i < numSymbols; i++)
                    prices[st + i] = b1 + this._highCoder.GetPrice(i - Base.kNumLowLenSymbols - Base.kNumMidLenSymbols);
            }
        };

        private class LenPriceTableEncoder : LenEncoder
        {
            private readonly UInt32[] _counters = new UInt32[Base.kNumPosStatesEncodingMax];
            private readonly UInt32[] _prices = new UInt32[Base.kNumLenSymbols << Base.kNumPosStatesBitsEncodingMax];
            private UInt32 _tableSize;

            private void UpdateTable(UInt32 posState)
            {
                this.SetPrices(posState, this._tableSize, this._prices, posState * Base.kNumLenSymbols);
                this._counters[posState] = this._tableSize;
            }

            public new void Encode(RangeCoder.Encoder rangeEncoder, UInt32 symbol, UInt32 posState)
            {
                base.Encode(rangeEncoder, symbol, posState);
                if (--this._counters[posState] == 0)
                    this.UpdateTable(posState);
            }

            public UInt32 GetPrice(UInt32 symbol, UInt32 posState) => this._prices[(posState * Base.kNumLenSymbols) + symbol];

            public void SetTableSize(UInt32 tableSize) => this._tableSize = tableSize;

            public void UpdateTables(UInt32 numPosStates)
            {
                for (UInt32 posState = 0; posState < numPosStates; posState++)
                    this.UpdateTable(posState);
            }
        }

        private class LiteralEncoder
        {
            private Encoder2[] m_Coders;

            private Int32 m_NumPosBits;

            private Int32 m_NumPrevBits;

            private UInt32 m_PosMask;

            public void Create(Int32 numPosBits, Int32 numPrevBits)
            {
                if (this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits)
                    return;
                this.m_NumPosBits = numPosBits;
                this.m_PosMask = ((UInt32)1 << numPosBits) - 1;
                this.m_NumPrevBits = numPrevBits;
                UInt32 numStates = (UInt32)1 << (this.m_NumPrevBits + this.m_NumPosBits);
                this.m_Coders = new Encoder2[numStates];
                for (UInt32 i = 0; i < numStates; i++)
                    this.m_Coders[i].Create();
            }

            public Encoder2 GetSubCoder(UInt32 pos, Byte prevByte) => this.m_Coders[((pos & this.m_PosMask) << this.m_NumPrevBits) + (UInt32)(prevByte >> (8 - this.m_NumPrevBits))];

            public void Init()
            {
                UInt32 numStates = (UInt32)1 << (this.m_NumPrevBits + this.m_NumPosBits);
                for (UInt32 i = 0; i < numStates; i++)
                    this.m_Coders[i].Init();
            }

            public struct Encoder2
            {
                private BitEncoder[] m_Encoders;

                public void Create() => this.m_Encoders = new BitEncoder[0x300];

                public void Encode(RangeCoder.Encoder rangeEncoder, Byte symbol)
                {
                    UInt32 context = 1;
                    for (Int32 i = 7; i >= 0; i--)
                    {
                        UInt32 bit = (UInt32)((symbol >> i) & 1);
                        this.m_Encoders[context].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public void EncodeMatched(RangeCoder.Encoder rangeEncoder, Byte matchByte, Byte symbol)
                {
                    UInt32 context = 1;
                    Boolean same = true;
                    for (Int32 i = 7; i >= 0; i--)
                    {
                        UInt32 bit = (UInt32)((symbol >> i) & 1);
                        UInt32 state = context;
                        if (same)
                        {
                            UInt32 matchBit = (UInt32)((matchByte >> i) & 1);
                            state += (1 + matchBit) << 8;
                            same = matchBit == bit;
                        }
                        this.m_Encoders[state].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public UInt32 GetPrice(Boolean matchMode, Byte matchByte, Byte symbol)
                {
                    UInt32 price = 0;
                    UInt32 context = 1;
                    Int32 i = 7;
                    if (matchMode)
                    {
                        for (; i >= 0; i--)
                        {
                            UInt32 matchBit = (UInt32)(matchByte >> i) & 1;
                            UInt32 bit = (UInt32)(symbol >> i) & 1;
                            price += this.m_Encoders[((1 + matchBit) << 8) + context].GetPrice(bit);
                            context = (context << 1) | bit;
                            if (matchBit != bit)
                            {
                                i--;
                                break;
                            }
                        }
                    }
                    for (; i >= 0; i--)
                    {
                        UInt32 bit = (UInt32)(symbol >> i) & 1;
                        price += this.m_Encoders[context].GetPrice(bit);
                        context = (context << 1) | bit;
                    }
                    return price;
                }

                public void Init()
                {
                    for (Int32 i = 0; i < 0x300; i++) this.m_Encoders[i].Init();
                }
            }
        }

        private class Optimal
        {
            public UInt32 BackPrev;
            public UInt32 BackPrev2;
            public UInt32 Backs0;
            public UInt32 Backs1;
            public UInt32 Backs2;
            public UInt32 Backs3;
            public UInt32 PosPrev;
            public UInt32 PosPrev2;
            public Boolean Prev1IsChar;
            public Boolean Prev2;
            public UInt32 Price;
            public Base.State State;

            public Boolean IsShortRep() => this.BackPrev == 0;

            public void MakeAsChar()
            {
                this.BackPrev = 0xFFFFFFFF; this.Prev1IsChar = false;
            }

            public void MakeAsShortRep()
            {
                this.BackPrev = 0; ; this.Prev1IsChar = false;
            }
        };
    }
}
