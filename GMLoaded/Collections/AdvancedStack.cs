using System;
using System.Collections;
using System.Diagnostics;

namespace GMLoaded.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    public class AdvancedStack<T> : IEnumerable
    {
        private T[] Values;

        public AdvancedStack(Int32 Count)
        {
            this.Values = new T[Count];
            this.Count = 0;
        }

        public AdvancedStack() : this(16)
        {
        }

        public Int32 Count { get; private set; }

        public T this[Int32 Idx]
        {
            get => Idx < 0 || Idx >= this.Count ? (default) : this.Values[Idx];

            set
            {
                if (Idx < 0 || Idx >= this.Count)
                    return;
                this.Values[Idx] = value;
            }
        }

        private void Upsize()
        {
            const Int32 LinearAfter = 65536;
            const Int32 LinearAmount = 1024;

            Int32 NewLen = this.Values.Length >= LinearAfter ? this.Values.Length + LinearAmount : this.Values.Length * 2;
            Array.Resize(ref this.Values, NewLen);
        }

        public void Clear()
        {
            for (Int32 i = 0; i < this.Values.Length; i++)
                this.Values[i] = default;
            this.Count = 0;
        }

        public IEnumerator GetEnumerator() => this.Values.GetEnumerator();

        public T Peek() => this.Count > 0 ? this.Values[this.Count - 1] : (default);

        public T Pop()
        {
            if (this.Count > 0)
            {
                T Ret = this.Values[--this.Count];
                this.Values[this.Count] = default;
                return Ret;
            }

            return default;
        }

        public T[] Pop(Int32 Num)
        {
            T[] Arr = new T[Num];
            for (Int32 i = 0; i < Num; i++)
                Arr[i] = this.Pop();
            return Arr;
        }

        public T PopBottom()
        {
            if (this.Count > 0)
            {
                T Ret = this.Values[0];
                for (Int32 i = 1; i < this.Count; i++)
                    this.Values[i - 1] = this.Values[i];
                this.Values[--this.Count] = default;
                return Ret;
            }

            return default;
        }

        public void Push(T Val)
        {
            if (this.Count >= this.Values.Length)
                this.Upsize();
            this.Values[this.Count++] = Val;
        }

        public void Push(params T[] Vals)
        {
            for (Int32 i = 0; i < Vals.Length; i++)
                this.Push(Vals[i]);
        }

        public void Reverse() => Array.Reverse(this.Values, 0, this.Count);

        public T[] ToArray()
        {
            T[] Arr = new T[this.Count];
            for (Int32 i = 0; i < this.Count; i++)
                Arr[i] = this.Values[i];
            return Arr;
        }
    }
}
