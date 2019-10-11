using System;
using System.Collections;
using System.Collections.Generic;

namespace GMLoaded.Source
{
    internal class StringTableEnumerator : IEnumerator<String>, IEnumerator
    {
        private Int32 position = -1;
        private StringTable table;

        public StringTableEnumerator(StringTable table) => this.table = table;

        public Object Current => this.table[this.position];

        String IEnumerator<String>.Current => this.table[this.position];

        public void Dispose() => this.table = null;

        public Boolean MoveNext() => ++this.position < this.table.Count();

        public void Reset() => this.position = -1;
    }
}
