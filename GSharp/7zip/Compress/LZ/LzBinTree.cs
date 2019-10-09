// LzBinTree.cs

using System;

namespace SevenZip.Compression.LZ
{
    public class BinTree : InWindow, IMatchFinder
    {
        private const UInt32 kBT2HashSize = 1 << 16;
        private const UInt32 kEmptyHashValue = 0;
        private const UInt32 kHash2Size = 1 << 10;
        private const UInt32 kHash3Offset = kHash2Size;
        private const UInt32 kHash3Size = 1 << 16;
        private const UInt32 kMaxValForNormalize = ((UInt32)1 << 31) - 1;
        private const UInt32 kStartMaxLen = 1;
        private UInt32 _cutValue = 0xFF;
        private UInt32 _cyclicBufferPos;
        private UInt32 _cyclicBufferSize = 0;
        private UInt32[] _hash;
        private UInt32 _hashMask;
        private UInt32 _hashSizeSum = 0;
        private UInt32 _matchMaxLen;

        private UInt32[] _son;
        private Boolean HASH_ARRAY = true;
        private UInt32 kFixHashSize = kHash2Size + kHash3Size;
        private UInt32 kMinMatchCheck = 4;
        private UInt32 kNumHashDirectBytes = 0;

        private void Normalize()
        {
            UInt32 subValue = this._pos - this._cyclicBufferSize;
            this.NormalizeLinks(this._son, this._cyclicBufferSize * 2, subValue);
            this.NormalizeLinks(this._hash, this._hashSizeSum, subValue);
            this.ReduceOffsets((Int32)subValue);
        }

        private void NormalizeLinks(UInt32[] items, UInt32 numItems, UInt32 subValue)
        {
            for (UInt32 i = 0; i < numItems; i++)
            {
                UInt32 value = items[i];
                if (value <= subValue)
                    value = kEmptyHashValue;
                else
                    value -= subValue;
                items[i] = value;
            }
        }

        public void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
                UInt32 matchMaxLen, UInt32 keepAddBufferAfter)
        {
            if (historySize > kMaxValForNormalize - 256)
                throw new Exception();
            this._cutValue = 16 + (matchMaxLen >> 1);

            UInt32 windowReservSize = ((historySize + keepAddBufferBefore +
                    matchMaxLen + keepAddBufferAfter) / 2) + 256;

            base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, windowReservSize);

            this._matchMaxLen = matchMaxLen;

            UInt32 cyclicBufferSize = historySize + 1;
            if (this._cyclicBufferSize != cyclicBufferSize)
                this._son = new UInt32[(this._cyclicBufferSize = cyclicBufferSize) * 2];

            UInt32 hs = kBT2HashSize;

            if (this.HASH_ARRAY)
            {
                hs = historySize - 1;
                hs |= hs >> 1;
                hs |= hs >> 2;
                hs |= hs >> 4;
                hs |= hs >> 8;
                hs >>= 1;
                hs |= 0xFFFF;
                if (hs > (1 << 24))
                    hs >>= 1;
                this._hashMask = hs;
                hs++;
                hs += this.kFixHashSize;
            }
            if (hs != this._hashSizeSum)
                this._hash = new UInt32[this._hashSizeSum = hs];
        }

        public new Byte GetIndexByte(Int32 index) => base.GetIndexByte(index);

        public UInt32 GetMatches(UInt32[] distances)
        {
            UInt32 lenLimit;
            if (this._pos + this._matchMaxLen <= this._streamPos)
                lenLimit = this._matchMaxLen;
            else
            {
                lenLimit = this._streamPos - this._pos;
                if (lenLimit < this.kMinMatchCheck)
                {
                    this.MovePos();
                    return 0;
                }
            }

            UInt32 offset = 0;
            UInt32 matchMinPos = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0;
            UInt32 cur = this._bufferOffset + this._pos;
            UInt32 maxLen = kStartMaxLen; // to avoid items for len < hashSize;
            UInt32 hashValue, hash2Value = 0, hash3Value = 0;

            if (this.HASH_ARRAY)
            {
                UInt32 temp = CRC.Table[this._bufferBase[cur]] ^ this._bufferBase[cur + 1];
                hash2Value = temp & (kHash2Size - 1);
                temp ^= (UInt32)this._bufferBase[cur + 2] << 8;
                hash3Value = temp & (kHash3Size - 1);
                hashValue = (temp ^ (CRC.Table[this._bufferBase[cur + 3]] << 5)) & this._hashMask;
            }
            else
                hashValue = this._bufferBase[cur] ^ ((UInt32)this._bufferBase[cur + 1] << 8);

            UInt32 curMatch = this._hash[this.kFixHashSize + hashValue];
            if (this.HASH_ARRAY)
            {
                UInt32 curMatch2 = this._hash[hash2Value];
                UInt32 curMatch3 = this._hash[kHash3Offset + hash3Value];
                this._hash[hash2Value] = this._pos;
                this._hash[kHash3Offset + hash3Value] = this._pos;
                if (curMatch2 > matchMinPos)
                    if (this._bufferBase[this._bufferOffset + curMatch2] == this._bufferBase[cur])
                    {
                        distances[offset++] = maxLen = 2;
                        distances[offset++] = this._pos - curMatch2 - 1;
                    }
                if (curMatch3 > matchMinPos)
                    if (this._bufferBase[this._bufferOffset + curMatch3] == this._bufferBase[cur])
                    {
                        if (curMatch3 == curMatch2)
                            offset -= 2;
                        distances[offset++] = maxLen = 3;
                        distances[offset++] = this._pos - curMatch3 - 1;
                        curMatch2 = curMatch3;
                    }
                if (offset != 0 && curMatch2 == curMatch)
                {
                    offset -= 2;
                    maxLen = kStartMaxLen;
                }
            }

            this._hash[this.kFixHashSize + hashValue] = this._pos;

            UInt32 ptr0 = (this._cyclicBufferPos << 1) + 1;
            UInt32 ptr1 = this._cyclicBufferPos << 1;

            UInt32 len0, len1;
            len0 = len1 = this.kNumHashDirectBytes;

            if (this.kNumHashDirectBytes != 0)
            {
                if (curMatch > matchMinPos)
                {
                    if (this._bufferBase[this._bufferOffset + curMatch + this.kNumHashDirectBytes] !=
                            this._bufferBase[cur + this.kNumHashDirectBytes])
                    {
                        distances[offset++] = maxLen = this.kNumHashDirectBytes;
                        distances[offset++] = this._pos - curMatch - 1;
                    }
                }
            }

            UInt32 count = this._cutValue;

            while (true)
            {
                if (curMatch <= matchMinPos || count-- == 0)
                {
                    this._son[ptr0] = this._son[ptr1] = kEmptyHashValue;
                    break;
                }
                UInt32 delta = this._pos - curMatch;
                UInt32 cyclicPos = ((delta <= this._cyclicBufferPos) ?
                            (this._cyclicBufferPos - delta) :
                            (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;

                UInt32 pby1 = this._bufferOffset + curMatch;
                UInt32 len = Math.Min(len0, len1);
                if (this._bufferBase[pby1 + len] == this._bufferBase[cur + len])
                {
                    while (++len != lenLimit)
                        if (this._bufferBase[pby1 + len] != this._bufferBase[cur + len])
                            break;
                    if (maxLen < len)
                    {
                        distances[offset++] = maxLen = len;
                        distances[offset++] = delta - 1;
                        if (len == lenLimit)
                        {
                            this._son[ptr1] = this._son[cyclicPos];
                            this._son[ptr0] = this._son[cyclicPos + 1];
                            break;
                        }
                    }
                }
                if (this._bufferBase[pby1 + len] < this._bufferBase[cur + len])
                {
                    this._son[ptr1] = curMatch;
                    ptr1 = cyclicPos + 1;
                    curMatch = this._son[ptr1];
                    len1 = len;
                }
                else
                {
                    this._son[ptr0] = curMatch;
                    ptr0 = cyclicPos;
                    curMatch = this._son[ptr0];
                    len0 = len;
                }
            }
            this.MovePos();
            return offset;
        }

        public new UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit) => base.GetMatchLen(index, distance, limit);

        public new UInt32 GetNumAvailableBytes() => base.GetNumAvailableBytes();

        public new void Init()
        {
            base.Init();
            for (UInt32 i = 0; i < this._hashSizeSum; i++)
                this._hash[i] = kEmptyHashValue;
            this._cyclicBufferPos = 0;
            this.ReduceOffsets(-1);
        }

        public new void MovePos()
        {
            if (++this._cyclicBufferPos >= this._cyclicBufferSize)
                this._cyclicBufferPos = 0;
            base.MovePos();
            if (this._pos == kMaxValForNormalize)
                this.Normalize();
        }

        public new void ReleaseStream() => base.ReleaseStream();

        public void SetCutValue(UInt32 cutValue) => this._cutValue = cutValue;

        public new void SetStream(System.IO.Stream stream) => base.SetStream(stream);

        public void SetType(Int32 numHashBytes)
        {
            this.HASH_ARRAY = numHashBytes > 2;
            if (this.HASH_ARRAY)
            {
                this.kNumHashDirectBytes = 0;
                this.kMinMatchCheck = 4;
                this.kFixHashSize = kHash2Size + kHash3Size;
            }
            else
            {
                this.kNumHashDirectBytes = 2;
                this.kMinMatchCheck = 2 + 1;
                this.kFixHashSize = 0;
            }
        }

        public void Skip(UInt32 num)
        {
            do
            {
                UInt32 lenLimit;
                if (this._pos + this._matchMaxLen <= this._streamPos)
                    lenLimit = this._matchMaxLen;
                else
                {
                    lenLimit = this._streamPos - this._pos;
                    if (lenLimit < this.kMinMatchCheck)
                    {
                        this.MovePos();
                        continue;
                    }
                }

                UInt32 matchMinPos = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0;
                UInt32 cur = this._bufferOffset + this._pos;

                UInt32 hashValue;

                if (this.HASH_ARRAY)
                {
                    UInt32 temp = CRC.Table[this._bufferBase[cur]] ^ this._bufferBase[cur + 1];
                    UInt32 hash2Value = temp & (kHash2Size - 1);
                    this._hash[hash2Value] = this._pos;
                    temp ^= (UInt32)this._bufferBase[cur + 2] << 8;
                    UInt32 hash3Value = temp & (kHash3Size - 1);
                    this._hash[kHash3Offset + hash3Value] = this._pos;
                    hashValue = (temp ^ (CRC.Table[this._bufferBase[cur + 3]] << 5)) & this._hashMask;
                }
                else
                    hashValue = this._bufferBase[cur] ^ ((UInt32)this._bufferBase[cur + 1] << 8);

                UInt32 curMatch = this._hash[this.kFixHashSize + hashValue];
                this._hash[this.kFixHashSize + hashValue] = this._pos;

                UInt32 ptr0 = (this._cyclicBufferPos << 1) + 1;
                UInt32 ptr1 = this._cyclicBufferPos << 1;

                UInt32 len0, len1;
                len0 = len1 = this.kNumHashDirectBytes;

                UInt32 count = this._cutValue;
                while (true)
                {
                    if (curMatch <= matchMinPos || count-- == 0)
                    {
                        this._son[ptr0] = this._son[ptr1] = kEmptyHashValue;
                        break;
                    }

                    UInt32 delta = this._pos - curMatch;
                    UInt32 cyclicPos = ((delta <= this._cyclicBufferPos) ?
                                (this._cyclicBufferPos - delta) :
                                (this._cyclicBufferPos - delta + this._cyclicBufferSize)) << 1;

                    UInt32 pby1 = this._bufferOffset + curMatch;
                    UInt32 len = Math.Min(len0, len1);
                    if (this._bufferBase[pby1 + len] == this._bufferBase[cur + len])
                    {
                        while (++len != lenLimit)
                            if (this._bufferBase[pby1 + len] != this._bufferBase[cur + len])
                                break;
                        if (len == lenLimit)
                        {
                            this._son[ptr1] = this._son[cyclicPos];
                            this._son[ptr0] = this._son[cyclicPos + 1];
                            break;
                        }
                    }
                    if (this._bufferBase[pby1 + len] < this._bufferBase[cur + len])
                    {
                        this._son[ptr1] = curMatch;
                        ptr1 = cyclicPos + 1;
                        curMatch = this._son[ptr1];
                        len1 = len;
                    }
                    else
                    {
                        this._son[ptr0] = curMatch;
                        ptr0 = cyclicPos;
                        curMatch = this._son[ptr0];
                        len0 = len;
                    }
                }
                this.MovePos();
            }
            while (--num != 0);
        }
    }
}
